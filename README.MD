# WindowOpacity

English | [中文](README.zh-CN.MD)

A lightweight Windows utility for adjusting window opacity with keyboard shortcuts. It runs on demand with no visible window, no tray icon, no background process, and no startup entry. It is mainly intended for **PowerToys Keyboard Manager**, but it can also be run manually.

## Files

| File | Description |
| --- | --- |
| `WindowOpacity.exe` | Recommended version. Starts quickly and works well with shortcuts. |
| `WindowOpacity.cs` | C# source code. If you do not trust the prebuilt exe, build it yourself from this file. |
| `Set-WindowOpacity.ps1` | PowerShell version. Easier to inspect, but much slower because it starts PowerShell and loads embedded C# each time. |

## Usage

```powershell
.\WindowOpacity.exe fg more-transparent 10
.\WindowOpacity.exe fg more-opaque 10
.\WindowOpacity.exe fg set 150
.\WindowOpacity.exe mouse set 180
```

Argument format:

```text
WindowOpacity.exe <target> <action> <value>
```

| Argument | Values | Description |
| --- | --- | --- |
| `target` | `fg`, `mouse` | Current foreground window, or the window under the mouse pointer. |
| `action` | `more-transparent`, `more-opaque`, `set` | Make the window more transparent, more opaque, or set alpha directly. |
| `value` | Number | For `set`, this is the target alpha. For `more-*`, this is the step size. Alpha range is `0` - `255`: `0` is fully transparent, `255` is fully opaque. |

## PowerToys Shortcut

Open:

```text
PowerToys -> Keyboard Manager -> Remap a shortcut
```

Recommended mappings:

| Shortcut | App | Arguments |
| --- | --- | --- |
| `Ctrl + Alt + -` | `WindowOpacity.exe` | `fg more-transparent 10` |
| `Ctrl + Alt + =` | `WindowOpacity.exe` | `fg more-opaque 10` |

Other options:

```text
Action: Start App
Visibility: Hidden
If running: Start another instance
```

## Build It Yourself

If you do not trust the included `WindowOpacity.exe`, compile it from `WindowOpacity.cs` in this directory.

Silent build, recommended for PowerToys:

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" `
  /nologo `
  /target:winexe `
  /optimize+ `
  /platform:x64 `
  /out:"WindowOpacity.exe" `
  "WindowOpacity.cs"
```

Debug build, with console output:

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" `
  /nologo `
  /target:exe `
  /optimize+ `
  /platform:x64 `
  /out:"WindowOpacity.exe" `
  "WindowOpacity.cs"
```

## PowerShell Version

You can also skip the exe and run the script directly:

```powershell
powershell.exe -NoProfile -File .\Set-WindowOpacity.ps1 -Action more-transparent -Step 25
powershell.exe -NoProfile -File .\Set-WindowOpacity.ps1 -Action more-opaque -Step 25
```

This version is easier to inspect, but it is not ideal for frequent shortcuts: every run starts PowerShell and loads embedded C# types, so the response is much slower than the exe.

## Notes

This tool uses native Windows APIs to change the window alpha value. When alpha is restored to `255`, the layered window style is removed. Some elevated or protected windows may require running the tool with matching privileges.

## License

MIT
