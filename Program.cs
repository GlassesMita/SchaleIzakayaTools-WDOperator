using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text;

namespace Schale_Izakaya_WD_Operator
{
    class Program
    {
        private const string AppTitle = "沙勒食堂工具：Windows Defender 操作工具";
        private const string ConfigFileName = "config.ini";
        private static string? cachedGamePath;
        private static readonly string AppDirectory;
        private static readonly string ConfigPath;

        static Program()
        {
            AppDirectory = AppContext.BaseDirectory;
            ConfigPath = Path.Combine(AppDirectory, ConfigFileName);
        }

        static void Main(string[] args)
        {
            Console.Title = AppTitle;
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            if (!IsRunningAsAdmin())
            {
                ShowError("请以管理员权限运行此应用程序！\n此应用程序需要管理员权限才能操作 Windows Defender。");
                return;
            }

            Console.WriteLine($"欢迎使用 {AppTitle}");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"应用程序目录: {AppDirectory}");

            LoadCachedPath();
            cachedGamePath ??= FindTouhouMystiaIzakaya();

            MainMenu();
        }

        static bool IsRunningAsAdmin()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        static void LoadCachedPath()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string[] lines = File.ReadAllLines(ConfigPath);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("GamePath=", StringComparison.OrdinalIgnoreCase))
                        {
                            string path = line.Substring(9).Trim();
                            if (Directory.Exists(path))
                            {
                                cachedGamePath = path;
                                Console.WriteLine($"已加载缓存的游戏路径: {path}");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载配置失败: {ex.Message}");
            }
        }

        static void SavePathToConfig(string path)
        {
            try
            {
                List<string> lines = new List<string>();
                if (File.Exists(ConfigPath))
                {
                    lines.AddRange(File.ReadAllLines(ConfigPath));
                }

                bool found = false;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].StartsWith("GamePath=", StringComparison.OrdinalIgnoreCase))
                    {
                        lines[i] = $"GamePath={path}";
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    lines.Add($"GamePath={path}");
                }

                File.WriteAllLines(ConfigPath, lines.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存配置失败: {ex.Message}");
            }
        }

        static string? FindTouhouMystiaIzakaya()
        {
            Console.WriteLine("\n正在查找 Touhou Mystia Izakaya 游戏目录...");

            string[] possibleGameDirs = {
                "Touhou Mystia's Izakaya",
                "Touhou Mystia Izakaya",
                "东方夜雀食堂",
                "Mystias Izakaya",
                "TouhouMystiaIzakaya_Data"
            };

            foreach (string dirName in possibleGameDirs)
            {
                string fullPath = Path.Combine(AppDirectory, dirName);
                if (Directory.Exists(fullPath))
                {
                    Console.WriteLine($"找到游戏目录: {fullPath}");
                    return fullPath;
                }

                string parentPath = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(parentPath) && Directory.Exists(parentPath))
                {
                    Console.WriteLine($"找到游戏目录: {parentPath}");
                    return parentPath;
                }
            }

            string parentDir = Directory.GetParent(AppDirectory)?.FullName;
            if (!string.IsNullOrEmpty(parentDir))
            {
                if (Directory.Exists(parentDir))
                {
                    string[] directories = Directory.GetDirectories(parentDir);
                    foreach (string dir in directories)
                    {
                        string dirName = Path.GetFileName(dir);
                        if (dirName.Contains("Mystia") || dirName.Contains("Izakaya") ||
                            dirName.Contains("夜雀") || dirName.Contains("食堂") ||
                            dirName.Contains("Touhou"))
                        {
                            Console.WriteLine($"找到游戏目录: {dir}");
                            return dir;
                        }
                    }
                }
            }

            string[] searchPaths = {
                @"D:\Games\Touhou Mystia's Izakaya",
                @"D:\Games\Touhou Mystia Izakaya",
                @"D:\Games\东方夜雀食堂",
                @"E:\Games\Touhou Mystia's Izakaya",
                @"E:\Games\Touhou Mystia Izakaya",
                @"C:\Games\Touhou Mystia's Izakaya"
            };

            foreach (string searchPath in searchPaths)
            {
                if (Directory.Exists(searchPath))
                {
                    Console.WriteLine($"找到游戏目录: {searchPath}");
                    return searchPath;
                }
            }

            Console.WriteLine("未在应用程序目录中找到 Touhou Mystia Izakaya 游戏目录！");
            Console.WriteLine("将使用手动输入模式。");

            return null;
        }

        static void MainMenu()
        {
            while (true)
            {
                Console.WriteLine("\n主菜单 - 请选择操作:");
                Console.WriteLine("[1] 禁用 Windows Defender");
                Console.WriteLine("[2] 启用 Windows Defender");
                Console.WriteLine("[3] 添加排除项（自动）");
                Console.WriteLine("[4] 添加排除项（手动）");
                Console.WriteLine("[5] 删除排除项");
                Console.WriteLine("[6] 查看当前游戏路径");
                Console.WriteLine("[7] 手动设置游戏路径");
                Console.WriteLine("[0] 退出应用");
                Console.Write("\n请输入选项: ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        DisableWindowsDefender();
                        break;
                    case "2":
                        EnableWindowsDefender();
                        break;
                    case "3":
                        AutoAddExclusions();
                        break;
                    case "4":
                        ManualAddExclusion();
                        break;
                    case "5":
                        RemoveExclusion();
                        break;
                    case "6":
                        ShowCurrentGamePath();
                        break;
                    case "7":
                        ManualSetGamePath();
                        break;
                    case "0":
                        Console.WriteLine("感谢使用，再见！");
                        return;
                    default:
                        ShowError("无效的选项，请重新输入！");
                        break;
                }
            }
        }

        static void ShowCurrentGamePath()
        {
            Console.WriteLine($"\n当前游戏路径: {cachedGamePath ?? "未设置"}");
        }

        static void ManualSetGamePath()
        {
            Console.WriteLine("\n手动设置游戏路径");
            Console.Write("请输入游戏目录路径: ");
            string? inputPath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(inputPath))
            {
                ShowError("路径不能为空！");
                return;
            }

            inputPath = inputPath.Trim('"', ' ');

            if (!Directory.Exists(inputPath))
            {
                ShowError("指定的路径不存在！");
                return;
            }

            cachedGamePath = inputPath;
            SavePathToConfig(inputPath);
            Console.WriteLine($"已设置并保存游戏路径: {cachedGamePath}");
        }

        static void DisableWindowsDefender()
        {
            Console.WriteLine("\n正在禁用 Windows Defender...");

            try
            {
                bool success1 = ExecutePowerShell("Set-MpPreference -DisableRealtimeMonitoring $true");
                bool success2 = ExecutePowerShell("Set-MpPreference -DisableBehaviorMonitoring $true");
                bool success3 = ExecutePowerShell("Set-MpPreference -DisableScriptScanning $true");

                if (success1 || success2 || success3)
                {
                    ShowSuccess("Windows Defender 已成功禁用！");
                }
                else
                {
                    ShowError("禁用 Windows Defender 失败！");
                }
            }
            catch (Exception ex)
            {
                ShowError($"禁用失败: {ex.Message}");
            }
        }

        static void EnableWindowsDefender()
        {
            Console.WriteLine("\n正在启用 Windows Defender...");

            try
            {
                bool success1 = ExecutePowerShell("Set-MpPreference -DisableRealtimeMonitoring $false");
                bool success2 = ExecutePowerShell("Set-MpPreference -DisableBehaviorMonitoring $false");
                bool success3 = ExecutePowerShell("Set-MpPreference -DisableScriptScanning $false");

                if (success1 && success2 && success3)
                {
                    ShowSuccess("Windows Defender 已成功启用！");
                }
                else
                {
                    ShowWarning("部分设置可能未能完全恢复，请手动检查。");
                }
            }
            catch (Exception ex)
            {
                ShowError($"启用失败: {ex.Message}");
            }
        }

        static void AutoAddExclusions()
        {
            Console.WriteLine("\n自动添加排除项");

            if (string.IsNullOrEmpty(cachedGamePath))
            {
                cachedGamePath = FindTouhouMystiaIzakaya();
            }

            if (string.IsNullOrEmpty(cachedGamePath))
            {
                ShowError("无法找到游戏路径！请先手动设置游戏路径（选项7）。");
                return;
            }

            Console.WriteLine($"\n游戏路径: {cachedGamePath}");
            Console.WriteLine("以下路径将被添加到排除列表:");

            List<string> excludePaths = GetExcludePaths(cachedGamePath);

            foreach (string path in excludePaths)
            {
                Console.WriteLine($"  - {path}");
            }

            Console.Write($"\n确认添加以上排除项? (Y/N): ");
            string? confirm = Console.ReadLine();

            if (string.IsNullOrEmpty(confirm) ||
                (!confirm.Equals("Y", StringComparison.OrdinalIgnoreCase) &&
                 !confirm.Equals("y", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("操作已取消。");
                return;
            }

            int successCount = 0;
            int failCount = 0;
            int skipCount = 0;

            foreach (string path in excludePaths)
            {
                if (!Directory.Exists(path) && !File.Exists(path))
                {
                    skipCount++;
                    continue;
                }

                string cleanPath = path.TrimEnd('\\', '/');
                string powershellCmd = $"Add-MpPreference -ExclusionPath '{cleanPath}'";

                try
                {
                    if (ExecutePowerShell(powershellCmd))
                    {
                        Console.WriteLine($"✓ 已添加: {Path.GetFileName(cleanPath)}");
                        successCount++;
                    }
                    else
                    {
                        Console.WriteLine($"✗ 失败: {Path.GetFileName(cleanPath)}");
                        failCount++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ 错误: {Path.GetFileName(path)} - {ex.Message}");
                    failCount++;
                }
            }

            Console.WriteLine($"\n完成! 成功: {successCount}, 失败: {failCount}, 跳过: {skipCount}");

            Console.Write("\n是否同时禁用 Windows Defender 实时保护? (Y/N): ");
            string? disableConfirm = Console.ReadLine();

            if (!string.IsNullOrEmpty(disableConfirm) &&
                (disableConfirm.Equals("Y", StringComparison.OrdinalIgnoreCase) ||
                 disableConfirm.Equals("y", StringComparison.OrdinalIgnoreCase)))
            {
                DisableWindowsDefender();
            }
        }

        static List<string> GetExcludePaths(string gamePath)
        {
            List<string> excludePaths = new List<string>();

            excludePaths.Add(AppDirectory);

            return excludePaths;
        }

        static void ManualAddExclusion()
        {
            Console.WriteLine("\n添加排除项（手动）");
            Console.WriteLine("排除类型: [1] 文件夹 [2] 文件 [3] 进程");
            Console.Write("请选择排除类型: ");

            string? typeInput = Console.ReadLine();
            if (string.IsNullOrEmpty(typeInput) || !int.TryParse(typeInput, out int type))
            {
                ShowError("无效的输入！");
                return;
            }

            Console.Write("请输入排除路径: ");
            string? path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path))
            {
                ShowError("路径不能为空！");
                return;
            }

            path = path.Trim('"', ' ');

            string powershellCmd;
            if (type == 3)
            {
                string processName = path;
                powershellCmd = $"Add-MpPreference -ExclusionProcess '{processName}'";
            }
            else
            {
                powershellCmd = $"Add-MpPreference -ExclusionPath '{path}'";
            }

            try
            {
                if (ExecutePowerShell(powershellCmd))
                {
                    ShowSuccess($"已成功添加排除项: {path}");
                }
                else
                {
                    ShowError("添加排除项失败！");
                }
            }
            catch (Exception ex)
            {
                ShowError($"添加失败: {ex.Message}");
            }
        }

        static void RemoveExclusion()
        {
            Console.WriteLine("\n删除排除项（白名单）");

            List<string> exclusions = GetExclusionList();

            if (exclusions.Count == 0)
            {
                ShowError("未找到任何排除项！");
                return;
            }

            Console.WriteLine("\n当前排除项列表:");
            Console.WriteLine("编号\t路径");
            Console.WriteLine(new string('-', 80));

            for (int i = 0; i < exclusions.Count; i++)
            {
                string displayPath = exclusions[i].Length > 70 
                    ? "..." + exclusions[i].Substring(exclusions[i].Length - 67)
                    : exclusions[i];
                Console.WriteLine($"[{i + 1}]\t{displayPath}");
            }

            Console.WriteLine(new string('-', 80));
            Console.Write("请输入要删除的排除项编号 (0 取消): ");

            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out int selection))
            {
                Console.WriteLine("取消操作。");
                return;
            }

            if (selection <= 0 || selection > exclusions.Count)
            {
                Console.WriteLine("取消操作或无效编号。");
                return;
            }

            string pathToDelete = exclusions[selection - 1];
            Console.WriteLine($"确认删除: {pathToDelete} (Y/N): ");

            string? confirm = Console.ReadLine();
            if (string.IsNullOrEmpty(confirm) ||
                (!confirm.Equals("Y", StringComparison.OrdinalIgnoreCase) &&
                 !confirm.Equals("y", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("已取消。");
                return;
            }

            string powershellCmd = $"Remove-MpPreference -ExclusionPath '{pathToDelete}'";

            try
            {
                if (ExecutePowerShell(powershellCmd))
                {
                    ShowSuccess($"已成功删除排除项!");
                }
                else
                {
                    ShowError("删除排除项失败！");
                }
            }
            catch (Exception ex)
            {
                ShowError($"删除失败: {ex.Message}");
            }
        }

        static List<string> GetExclusionList()
        {
            List<string> exclusions = new List<string>();

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "powershell";
                psi.Arguments = "-Command \"Get-MpPreference | Select-Object -ExpandProperty ExclusionPath\"";
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.CreateNoWindow = true;

                using (Process? process = Process.Start(psi))
                {
                    if (process == null) return exclusions;

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                    {
                        string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string line in lines)
                        {
                            string trimmed = line.Trim();
                            if (!string.IsNullOrEmpty(trimmed))
                            {
                                exclusions.Add(trimmed);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取排除列表失败: {ex.Message}");
            }

            exclusions.Sort();
            return exclusions;
        }

        static bool ExecutePowerShell(string command)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "powershell";
                psi.Arguments = $"-Command \"{command}\"";
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.CreateNoWindow = true;

                using (Process? process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        Console.WriteLine("错误: 无法启动 PowerShell 进程");
                        return false;
                    }

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine($"PowerShell 错误: {error}");
                    }

                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PowerShell 执行错误: {ex.Message}");
                return false;
            }
        }

        static void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {message}");
            Console.ResetColor();
        }

        static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {message}");
            Console.ResetColor();
        }

        static void ShowWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {message}");
            Console.ResetColor();
        }
    }
}
