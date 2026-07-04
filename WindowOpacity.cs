using System;
using System.Runtime.InteropServices;
using System.Text;

internal static class Program
{
    private const int GWL_EXSTYLE = -20;
    private const long WS_EX_LAYERED = 0x00080000L;
    private const uint LWA_ALPHA = 0x00000002;

    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_FRAMECHANGED = 0x0020;

    private const uint GA_ROOT = 2;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr WindowFromPoint(POINT point);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetLayeredWindowAttributes(
        IntPtr hwnd,
        out uint crKey,
        out byte bAlpha,
        out uint dwFlags
    );

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetLayeredWindowAttributes(
        IntPtr hwnd,
        uint crKey,
        byte bAlpha,
        uint dwFlags
    );

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags
    );

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowTextW(
        IntPtr hWnd,
        StringBuilder lpString,
        int nMaxCount
    );

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetClassNameW(
        IntPtr hWnd,
        StringBuilder lpClassName,
        int nMaxCount
    );

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    public static int Main(string[] args)
    {
        // 用法：
        // WindowOpacity.exe fg set 150
        // WindowOpacity.exe fg more-transparent 25
        // WindowOpacity.exe fg more-opaque 25
        // WindowOpacity.exe mouse set 150

        string targetMode = args.Length > 0 ? args[0].ToLowerInvariant() : "fg";
        string action = args.Length > 1 ? args[1].ToLowerInvariant() : "set";

        int value = 150;
        if (args.Length > 2)
        {
            int.TryParse(args[2], out value);
        }

        IntPtr hwnd;

        if (targetMode == "mouse")
        {
            POINT pt;

            if (!GetCursorPos(out pt))
            {
                Console.WriteLine("GetCursorPos failed: " + Marshal.GetLastWin32Error());
                return 10;
            }

            hwnd = WindowFromPoint(pt);
            hwnd = GetAncestor(hwnd, GA_ROOT);
        }
        else
        {
            hwnd = GetForegroundWindow();
            hwnd = GetAncestor(hwnd, GA_ROOT);
        }

        if (hwnd == IntPtr.Zero)
        {
            Console.WriteLine("No window found.");
            return 1;
        }

        Console.WriteLine("Target HWND: 0x" + hwnd.ToInt64().ToString("X"));
        Console.WriteLine("Title: " + GetTitle(hwnd));
        Console.WriteLine("Class: " + GetClass(hwnd));

        uint crKey;
        uint flags;
        byte currentAlpha;

        bool hasAlpha = GetLayeredWindowAttributes(hwnd, out crKey, out currentAlpha, out flags);
        int layeredErr = Marshal.GetLastWin32Error();

        if (!hasAlpha || (flags & LWA_ALPHA) == 0)
        {
            currentAlpha = 255;
        }

        Console.WriteLine("Current alpha: " + currentAlpha);
        Console.WriteLine("Has alpha: " + hasAlpha + ", flags: " + flags + ", err: " + layeredErr);

        int minAlpha = 0;
        int maxAlpha = 255;
        int newAlpha;

        switch (action)
        {
            case "set":
                newAlpha = value;
                break;

            case "more-transparent":
            case "transparent":
            case "-":
                if (currentAlpha <= minAlpha)
                {
                    Console.WriteLine("Already at minimum alpha. Nothing to do.");
                    return 0;
                }

                newAlpha = currentAlpha - value;

                if (newAlpha < minAlpha)
                {
                    newAlpha = minAlpha;
                }

                break;

            case "more-opaque":
            case "opaque":
            case "+":
                if (currentAlpha >= maxAlpha)
                {
                    Console.WriteLine("Already fully opaque. Nothing to do.");
                    return 0;
                }

                newAlpha = currentAlpha + value;

                if (newAlpha > maxAlpha)
                {
                    newAlpha = maxAlpha;
                }

                break;

            default:
                Console.WriteLine("Unknown action: " + action);
                return 2;
        }

        if (newAlpha < minAlpha) newAlpha = minAlpha;
        if (newAlpha > maxAlpha) newAlpha = maxAlpha;

        if (newAlpha == currentAlpha)
        {
            Console.WriteLine("Alpha unchanged. Nothing to do.");
            return 0;
        }

        Console.WriteLine("New alpha: " + newAlpha);

        long style = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
        Console.WriteLine("Old exstyle: 0x" + style.ToString("X"));

        if (newAlpha >= 255)
        {
            long normalStyle = style & ~WS_EX_LAYERED;

            SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(normalStyle));
            int err1 = Marshal.GetLastWin32Error();

            bool posOk = SetWindowPos(
                hwnd,
                IntPtr.Zero,
                0,
                0,
                0,
                0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED
            );

            Console.WriteLine("SetWindowLongPtr remove layered err: " + err1);
            Console.WriteLine("SetWindowPos ok: " + posOk + ", err: " + Marshal.GetLastWin32Error());
            return 0;
        }

        long layeredStyle = style | WS_EX_LAYERED;

        SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(layeredStyle));
        int err2 = Marshal.GetLastWin32Error();

        Console.WriteLine("SetWindowLongPtr add layered err: " + err2);
        Console.WriteLine("New exstyle: 0x" + layeredStyle.ToString("X"));

        bool ok = SetLayeredWindowAttributes(hwnd, 0, (byte)newAlpha, LWA_ALPHA);
        int err3 = Marshal.GetLastWin32Error();

        Console.WriteLine("SetLayeredWindowAttributes ok: " + ok + ", err: " + err3);

        bool posOk2 = SetWindowPos(
            hwnd,
            IntPtr.Zero,
            0,
            0,
            0,
            0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED
        );

        Console.WriteLine("SetWindowPos ok: " + posOk2 + ", err: " + Marshal.GetLastWin32Error());

        return ok ? 0 : 3;
    }

    private static string GetTitle(IntPtr hwnd)
    {
        StringBuilder sb = new StringBuilder(512);
        GetWindowTextW(hwnd, sb, sb.Capacity);
        return sb.ToString();
    }

    private static string GetClass(IntPtr hwnd)
    {
        StringBuilder sb = new StringBuilder(256);
        GetClassNameW(hwnd, sb, sb.Capacity);
        return sb.ToString();
    }
}
