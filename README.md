# 沙勒食堂工具 - Windows Defender 操作工具

![.NET Version](https://img.shields.io/badge/.NET-8.0-blue?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-Windows%20x64-green?style=flat-square)
![License](https://img.shields.io/badge/license-GNU%20GPL%20v3-red.svg?style=flat-square)

一个专为《东方夜雀食堂》（Touhou Mystia's Izakaya）玩家设计的 Windows Defender 管理工具，帮助您快速配置游戏目录的排除规则，确保游戏运行时不会因为 Mod 或游戏文件被 Windows Defender 误杀。

## 功能特点

### 核心功能
- **一键禁用 Windows Defender**：快速关闭实时保护、行为监控和脚本扫描
- **一键启用 Windows Defender**：随时恢复安全设置
- **自动添加排除项**：扫描游戏目录，自动添加相关路径到排除列表
- **智能路径识别**：自动检测游戏安装路径，支持多种安装位置
- **路径缓存保存**：首次设置后自动保存游戏路径，下次启动无需重新设置
- **删除排除项**：可视化界面选择要删除的排除项

### 设计理念
- **安全优先**：需要管理员权限才能运行，确保成功操作 Windows Defender
- **自动查找**：启动时自动扫描游戏目录，无需手动配置

## 系统要求

| 项目 | 要求 |
|------|------|
| 操作系统 | Windows 10 / Windows 11 |
| 架构 | x64 (64位) |
| .NET 运行时 | .NET 8.0 或更高版本 |
| 权限 | 管理员权限（必需） |
| PowerShell | Windows PowerShell 5.1+ |

## 下载与安装

### 方法一：直接下载（推荐）

1. 从 [Releases 页面](https://github.com/GlassesMita/SchaleIzakayaTools-WDOperator/releases) 下载最新版本
2. 解压到任意目录
3. 运行 `沙勒食堂工具：Windows Defender 操作工具.exe`

### 方法二：从源码编译

```bash
# 克隆仓库
git clone https://github.com/GlassesMita/SchaleIzakayaTools-WDOperator.git
cd SchaleIzakayaTools-WDOperator

# 发布 Release 版本
dotnet publish -c Release -o publish
```

## 使用方法

### 启动程序

**重要**：程序需要管理员权限才能操作 Windows Defender。

1. 双击运行程序
2. UAC 提示时选择 **"是"** 以管理员身份运行

### 主菜单说明

```
欢迎使用 沙勒食堂工具：Windows Defender 操作工具
============================================================
应用程序目录: C:\Program Files\SchaleIzakayaTools-WDOperator

主菜单 - 请选择操作:
[1] 禁用 Windows Defender
[2] 启用 Windows Defender
[3] 添加排除项（自动）
[4] 添加排除项（手动）
[5] 删除排除项
[6] 查看当前游戏路径
[7] 手动设置游戏路径
[0] 退出应用

请输入选项: _
```

### 功能详解

#### [1] 禁用 Windows Defender
一次性关闭以下保护：
- 实时监控（Real-time Monitoring）
- 行为监控（Behavior Monitoring）
- 脚本扫描（Script Scanning）

#### [2] 启用 Windows Defender
恢复上述所有安全设置。

#### [3] 添加排除项（自动）推荐
自动扫描游戏目录，将以下内容添加到排除列表：
- 游戏主目录
- 可执行文件
- Unity 数据目录
- 运行时库文件

**首次使用流程**：
1. 选择 [3]
2. 程序自动查找游戏路径（支持多种安装位置）
3. 显示将要添加的排除项列表
4. 确认后自动执行

#### [4] 添加排除项（手动）
手动添加特定文件、文件夹或进程到排除列表。

#### [5] 删除排除项
显示当前所有排除项，按编号选择删除。

#### [6] 查看当前游戏路径
显示已缓存的游戏安装路径。

#### [7] 手动设置游戏路径
手动输入游戏目录路径并保存。

## 游戏目录识别

### 自动查找顺序

程序启动时会自动按以下顺序查找游戏目录：

1. **Steam 注册表路径**：读取 Steam 安装位置，查找 `steamapps\common` 目录
2. **常见 Steam 路径**：扫描各磁盘根目录下的 `SteamLibrary\steamapps\common`
3. **程序父目录**：搜索运行目录的上一级目录
4. **程序目录**：搜索程序自身所在目录

### 验证条件

程序通过以下条件验证找到的目录是否为游戏目录：
- 目录中存在 `.exe` 可执行文件
- 目录中存在 `UnityPlayer.dll` 文件
- 目录中存在 `<可执行文件名>_Data` 目录

### 支持的文件夹名称

程序支持多种游戏文件夹命名方式：
- Touhou Mystia's Izakaya
- Touhou Mystia Izakaya
- 东方夜雀食堂
- Mystia's Izakaya
- 其他包含游戏文件的目录

## 编译说明

### 环境要求

- .NET 8.0 SDK 或更高版本
- Visual Studio 2022 或 VS Code（可选）

### 编译步骤

```bash
# 进入项目目录
cd SchaleIzakayaTools-WDOperator

# Debug 版本
dotnet build -c Debug

# Release 版本（推荐）
dotnet publish -c Release -o publish

# 清理构建
dotnet clean
```

### 发布文件说明

发布后会生成以下文件：

```
publish/
├── 沙勒食堂工具：Windows Defender 操作工具.exe     # 主程序
├── 沙勒食堂工具：Windows Defender 操作工具.dll     # 运行时库
├── 沙勒食堂工具：Windows Defender 操作工具.pdb     # 调试信息
├── 沙勒食堂工具：Windows Defender 操作工具.deps.json
├── 沙勒食堂工具：Windows Defender 操作工具.runtimeconfig.json
└── config.ini                                        # 配置文件（运行时生成）
```

**注意**：需要保持所有文件在同一目录才能正常运行。

## 常见问题

### Q: 程序无法启动？
**A**: 请确保以管理员权限运行程序。双击程序后，UAC 提示时选择"是"。

### Q: 添加排除项失败？
**A**:
1. 检查是否以管理员权限运行
2. 确保游戏路径正确且存在
3. 检查 PowerShell 是否正常工作

### Q: 游戏路径识别不到？
**A**:
1. 尝试手动设置游戏路径（选项 7）
2. 确保游戏文件夹名称符合支持的标准命名
3. 检查目录权限
4. 确保目录包含游戏必需文件（UnityPlayer.dll 和 *_Data 目录）

### Q: 如何完全卸载？
**A**:
1. 删除程序文件
2. 如果需要，删除 Windows Defender 中的排除项（使用选项 5）
3. 删除配置文件 config.ini

### Q: 支持其他游戏吗？
**A**: 当前版本专为《东方夜雀食堂》优化，但手动添加排除项功能可用于任何游戏或程序。

### Q: 安全性如何？
**A**:
- 程序不收集任何用户数据
- 所有操作都需要管理员权限确认
- 配置文件仅存储游戏路径

## 许可证

本项目采用 GNU General Public License v3.0 协议。
