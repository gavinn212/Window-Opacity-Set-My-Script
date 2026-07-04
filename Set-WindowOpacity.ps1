param(
    [ValidateSet("more-transparent", "more-opaque")]
    [string]$Action,

    [int]$Step = 25,
    [int]$MinAlpha = 70
)

if (-not ("WinOpacityNative2" -as [type])) {
    Add-Type -TypeDefinition @"
using System;
using System.Runtime.InteropServices;

public static class WinOpacityNative2 {
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", EntryPoint="GetWindowLongPtrW")]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint="SetWindowLongPtrW")]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll")]
    public static extern bool SetLayeredWindowAttributes(
        IntPtr hwnd,
        uint crKey,
        byte bAlpha,
        uint dwFlags
    );

    [DllImport("user32.dll")]
    public static extern bool GetLayeredWindowAttributes(
        IntPtr hwnd,
        out uint crKey,
        out byte bAlpha,
        out uint dwFlags
    );

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags
    );
}
"@
}

$GWL_EXSTYLE = -20
$WS_EX_LAYERED = 0x00080000
$LWA_ALPHA = 0x00000002

$SWP_NOSIZE = 0x0001
$SWP_NOMOVE = 0x0002
$SWP_NOZORDER = 0x0004
$SWP_FRAMECHANGED = 0x0020
$SWP_NOACTIVATE = 0x0010

$hwnd = [WinOpacityNative2]::GetForegroundWindow()
if ($hwnd -eq [IntPtr]::Zero) {
    exit
}

$crKey = 0
$currentAlpha = [byte]255
$flags = 0

$ok = [WinOpacityNative2]::GetLayeredWindowAttributes(
    $hwnd,
    [ref]$crKey,
    [ref]$currentAlpha,
    [ref]$flags
)

if (-not $ok -or (($flags -band $LWA_ALPHA) -eq 0)) {
    $currentAlpha = [byte]255
}

switch ($Action) {
    "more-transparent" {
        $newAlpha = [int]$currentAlpha - $Step
        if ($newAlpha -lt $MinAlpha) {
            $newAlpha = $MinAlpha
        }
    }

    "more-opaque" {
        $newAlpha = [int]$currentAlpha + $Step
        if ($newAlpha -gt 255) {
            $newAlpha = 255
        }
    }
}

$style = [WinOpacityNative2]::GetWindowLongPtr($hwnd, $GWL_EXSTYLE).ToInt64()

if ($newAlpha -ge 255) {
    # 到 255 时直接移除 layered 样式，恢复成普通不透明窗口
    $newStyle = $style -band (-bnot $WS_EX_LAYERED)

    [WinOpacityNative2]::SetWindowLongPtr(
        $hwnd,
        $GWL_EXSTYLE,
        [IntPtr]$newStyle
    ) | Out-Null

    [WinOpacityNative2]::SetWindowPos(
        $hwnd,
        [IntPtr]::Zero,
        0,
        0,
        0,
        0,
        $SWP_NOMOVE -bor $SWP_NOSIZE -bor $SWP_NOZORDER -bor $SWP_FRAMECHANGED -bor $SWP_NOACTIVATE
    ) | Out-Null

    exit
}

$newStyle = $style -bor $WS_EX_LAYERED

[WinOpacityNative2]::SetWindowLongPtr(
    $hwnd,
    $GWL_EXSTYLE,
    [IntPtr]$newStyle
) | Out-Null

[WinOpacityNative2]::SetLayeredWindowAttributes(
    $hwnd,
    0,
    [byte]$newAlpha,
    $LWA_ALPHA
) | Out-Null