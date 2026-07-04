# WindowOpacity

English | [中文](#中文)

A small Windows utility for changing the opacity of the current active window.

Designed to work with **Microsoft PowerToys Keyboard Manager**, but should work just fine with any other program supporting running exe.

## Features

* Adjust opacity of the active foreground window
* Make a window more transparent or more opaque
* Set a fixed opacity value
* Supports opacity range `0` to `255`
* No background process
* No AutoHotkey required
* Uses native Windows APIs

## Opacity Values

```text
0   = fully transparent
255 = fully opaque
```

## Usage

```powershell
WindowOpacity.exe fg more-transparent 10
WindowOpacity.exe fg more-opaque 10
WindowOpacity.exe fg set 150
WindowOpacity.exe fg set 255
```

## Arguments

```text
WindowOpacity.exe <target> <action> <value>
```

### Target

| Target  | Description                      |
| ------- | -------------------------------- |
| `fg`    | Current active foreground window |
| `mouse` | Window under the mouse pointer   |

### Action

| Action             | Description                |
| ------------------ | -------------------------- |
| `more-transparent` | Decrease opacity alpha     |
| `more-opaque`      | Increase opacity alpha     |
| `set`              | Set opacity alpha directly |

## PowerToys Setup

Open:

```text
PowerToys -> Keyboard Manager -> Remap a shortcut
```

Recommended mappings:

| Shortcut         | App                 | Arguments                |
| ---------------- | ------------------- | ------------------------ |
| `Ctrl + Alt + -` | `WindowOpacity.exe` | `fg more-transparent 10` |
| `Ctrl + Alt + =` | `WindowOpacity.exe` | `fg more-opaque 10`      |

Set:

```text
Action: Start App
Visibility: Hidden
If running: Start another instance
```

## Build

### Debug build

Shows console output.

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" `
  /nologo `
  /target:exe `
  /optimize+ `
  /platform:x64 `
  /out:"WindowOpacity.exe" `
  "WindowOpacity.cs"
```

### Silent build

Recommended for PowerToys.

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" `
  /nologo `
  /target:winexe `
  /optimize+ `
  /platform:x64 `
  /out:"WindowOpacity.exe" `
  "WindowOpacity.cs"
```

## Notes

The tool applies the `WS_EX_LAYERED` window style and updates the window alpha value using native Windows APIs.

When opacity is set back to `255`, the layered style is removed.

Some elevated or protected windows may require matching administrator privileges.

## License

MIT

---

# 中文

一个用于调整当前活动窗口透明度的小型 Windows 工具。

为 **Microsoft PowerToys Keyboard Manager** 设计，但可以适用于任何支持exe的软件。

## 功能

* 调整当前活动窗口的透明度
* 让窗口更透明或更不透明
* 支持直接设置固定透明度
* 支持透明度范围 `0` 到 `255`
* 无后台常驻进程
* 不需要 AutoHotkey
* 使用 Windows 原生 API

## 透明度数值

```text
0   = 完全透明
255 = 完全不透明
```

## 使用方式

```powershell
WindowOpacity.exe fg more-transparent 10
WindowOpacity.exe fg more-opaque 10
WindowOpacity.exe fg set 150
WindowOpacity.exe fg set 255
```

## 参数

```text
WindowOpacity.exe <目标> <操作> <数值>
```

### 目标

| 目标      | 说明        |
| ------- | --------- |
| `fg`    | 当前活动前台窗口  |
| `mouse` | 鼠标指针下方的窗口 |

### 操作

| 操作                 | 说明               |
| ------------------ | ---------------- |
| `more-transparent` | 降低 alpha，让窗口更透明  |
| `more-opaque`      | 提高 alpha，让窗口更不透明 |
| `set`              | 直接设置 alpha 值     |

## PowerToys 设置

打开：

```text
PowerToys -> Keyboard Manager -> Remap a shortcut
```

推荐映射：

| 快捷键              | App                 | Arguments                |
| ---------------- | ------------------- | ------------------------ |
| `Ctrl + Alt + -` | `WindowOpacity.exe` | `fg more-transparent 10` |
| `Ctrl + Alt + =` | `WindowOpacity.exe` | `fg more-opaque 10`      |

设置：

```text
Action: Start App
Visibility: Hidden
If running: Start another instance
```

## 编译

### 调试版

会显示控制台输出。

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" `
  /nologo `
  /target:exe `
  /optimize+ `
  /platform:x64 `
  /out:"WindowOpacity.exe" `
  "WindowOpacity.cs"
```

### 静默版

推荐给 PowerToys 使用。

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" `
  /nologo `
  /target:winexe `
  /optimize+ `
  /platform:x64 `
  /out:"WindowOpacity.exe" `
  "WindowOpacity.cs"
```

## 注意事项

本工具通过添加 `WS_EX_LAYERED` 窗口样式，并使用 Windows 原生 API 修改窗口 alpha 值。

当透明度恢复到 `255` 时，会移除 layered 样式，让窗口恢复普通不透明状态。

部分管理员权限窗口或受保护窗口可能需要同等管理员权限才能修改。

## License

MIT
