# WindowOpacity

[English](README.MD) | 中文

用快捷键调整 Windows 窗口透明度的轻量小工具。按需运行，无可见窗口、无托盘图标、无后台常驻、无需开机自启动。主要配合 **PowerToys Keyboard Manager** 使用，也可以手动运行。

## 文件

| 文件 | 说明 |
| --- | --- |
| `WindowOpacity.exe` | 推荐使用，启动快，适合绑定快捷键。 |
| `WindowOpacity.cs` | C# 源码；不放心预编译 exe 时，可以自己从源码编译。 |
| `Set-WindowOpacity.ps1` | PowerShell 版本；源码更容易审查，但每次启动/加载都更慢，快捷键响应会差很多。 |

## 用法

```powershell
.\WindowOpacity.exe fg more-transparent 10
.\WindowOpacity.exe fg more-opaque 10
.\WindowOpacity.exe fg set 150
.\WindowOpacity.exe mouse set 180
```

参数格式：

```text
WindowOpacity.exe <target> <action> <value>
```

| 参数 | 可选值 | 说明 |
| --- | --- | --- |
| `target` | `fg`, `mouse` | 当前前台窗口，或鼠标指针下方窗口。 |
| `action` | `more-transparent`, `more-opaque`, `set` | 调透明、调不透明，或直接设置 alpha。 |
| `value` | 数字 | `set` 时是目标 alpha；`more-*` 时是步进值。alpha 范围为 `0` - `255`，`0` 完全透明，`255` 完全不透明。 |

## PowerToys 快捷键

打开：

```text
PowerToys -> Keyboard Manager -> Remap a shortcut
```

推荐设置：

| 快捷键 | App | Arguments |
| --- | --- | --- |
| `Ctrl + Alt + -` | `WindowOpacity.exe` | `fg more-transparent 10` |
| `Ctrl + Alt + =` | `WindowOpacity.exe` | `fg more-opaque 10` |

其他选项：

```text
Action: Start App
Visibility: Hidden
If running: Start another instance
```

## 自己编译 exe

如果不放心仓库里的 `WindowOpacity.exe`，可以在当前目录从 `WindowOpacity.cs` 编译。

静默版，推荐给 PowerToys：

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" `
  /nologo `
  /target:winexe `
  /optimize+ `
  /platform:x64 `
  /out:"WindowOpacity.exe" `
  "WindowOpacity.cs"
```

调试版，会显示控制台输出：

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" `
  /nologo `
  /target:exe `
  /optimize+ `
  /platform:x64 `
  /out:"WindowOpacity.exe" `
  "WindowOpacity.cs"
```

## PowerShell 版本

也可以不用 exe，直接运行脚本：

```powershell
powershell.exe -NoProfile -File .\Set-WindowOpacity.ps1 -Action more-transparent -Step 25
powershell.exe -NoProfile -File .\Set-WindowOpacity.ps1 -Action more-opaque -Step 25
```

这个版本更容易审查源码，但不适合高频快捷键：每次都要启动 PowerShell，并加载内嵌 C# 类型，响应会比 exe 慢很多。

## 注意

本工具通过 Windows 原生 API 修改窗口 alpha；当 alpha 恢复到 `255` 时，会移除 layered 样式。部分管理员权限窗口或受保护窗口需要用同等权限运行工具才能修改。

## License

MIT
