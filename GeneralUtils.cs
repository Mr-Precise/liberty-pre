using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace libertypre
{
    public class GeneralUtils
    {
        // Выполнение команды с захватом вывода
        // Execute command with output capture
        public static string RunCommandReadToEnd(string command, string args)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process process = Process.Start(psi);
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }

        // Выполнение команды от имени администратора
        // Execute command as administrator
        private static void RunWindowsCommandAsAdmin(string command, string args)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                UseShellExecute = true,     // Требуется для runas / Required for runas
                Verb = "runas",             // Запуск с повышенными привилегиями / Run with elevated privileges
                CreateNoWindow = true
            };
            Process.Start(psi);
        }

        // Остановка и удаление служб / драйверов
        // Stop and remove services / drivers
        public static void StopRemoveWidowsSevice()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("winws");
                foreach (Process proc in processes)
                {
                    // Принудительно завершаем процесс winws
                    // Force terminate winws process
                    proc.Kill();
                    proc.WaitForExit(1000);
                }

                // Останавливаем и удаляем службы через cmd (да, так не красиво делать...)
                // Stop and delete services via cmd (yeah, it's not good to do that...)
                RunWindowsCommandAsAdmin("cmd", $"/c net stop WinDivert & sc delete WinDivert & net stop WinDivert14 & sc delete WinDivert14 & net stop Monkey & sc delete Monkey");
                LocaleUtils.WriteTr("StopRemoveDrv");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Thread.Sleep(3000); // Пауза на всякий случай / Pause just in case
        }

        // Проверка запущен ли процесс
        // Check if process is running
        public static bool IsProcessRunningBool(string processName)
        {
            if (IsLinux())
            {
                try
                {
                    // Используем pgrep -x для точного поиска процесса по имени
                    // Use pgrep -x for exact process name search
                    string pgrepOutput = RunCommandReadToEnd("pgrep", $"-x {processName}");
                    return !string.IsNullOrEmpty(pgrepOutput.Trim());
                }
                catch
                {
                    return false;
                }
            }
            else if (IsMacOS())
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "ps",
                        Arguments = "-ax", // a - все пользователи, x - процессы без терминала / a - all users, x - processes without terminal
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    var process = Process.Start(startInfo);
                    if (process == null)
                    {
                        return false;
                    }

                    // Читаем весь вывод команды ps
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    // Ищем имя процесса в общем списке
                    // но процессы часто ищут без учета регистра.
                    // StringComparison.OrdinalIgnoreCase полезен, так как macOS регистрозависима в путях, 
                    // Look for the process name in the overall list
                    // StringComparison.OrdinalIgnoreCase is useful since macOS is case-sensitive in paths,
                    // but processes are often searched case-insensitively.
                    return output.IndexOf(processName, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                Process[] processes = Process.GetProcessesByName(processName);
                return processes.Length > 0;
            }
        }

        // Выполнение команды через sudo
        // Execute command via sudo
        public static bool RunLinuxSudoCommandReadToEnd(string command)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "sudo",
                        Arguments = command,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = System.Text.Encoding.UTF8,
                        StandardErrorEncoding = System.Text.Encoding.UTF8
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit(3000);

                if (process.ExitCode != 0)
                {
                    if (!string.IsNullOrEmpty(error))
                        LocaleUtils.WriteTr("ErrorCommandFailed", command, error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LocaleUtils.WriteTr("ErrorSudoCommandFailed", command, ex.Message);
                return false;
            }
        }

        // Проверка наличия команды в Linux
        // Check for command availability in Linux
        public static bool UnixCommandExists(string command)
        {
            return RunCommandReadToEnd("which", command).Length > 0;
        }

        // Проверка, работает ли система под Linux
        // Check if system is running Linux
        public static bool IsLinux()
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                return false;
            }
            // Проверяем наличие файла, характерного для Linux
            // Check for a file characteristic of Linux
            return File.Exists("/proc/sys/kernel/ostype");
        }

        // Проверка, работает ли система под macOS
        // Check if system is running macOS
        public static bool IsMacOS()
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                return false;
            }
            // Проверяем наличие каталога, характерного для macOS
            // Check for a directory characteristic of macOS
            return Directory.Exists("/System/Library");
        }
    }
}
