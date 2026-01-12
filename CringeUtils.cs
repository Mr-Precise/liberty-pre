using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace libertypre
{
    public class CringeUtils
    {
        // Событие завершения создания ярлыков
        // Shortcut creation completion event
        public static ManualResetEvent ShortcutCrDone = new ManualResetEvent(false);
        private static string selfPath = Assembly.GetExecutingAssembly().Location;

        // Выполнение команды с захватом вывода
        // Execute command with output capture
        public static string RunCommand(string command, string args)
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
        private static void RunCommandAsAdmin(string command, string args)
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
        public static void StopRemoveSevice()
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
                RunCommandAsAdmin("cmd", $"/c net stop WinDivert & sc delete WinDivert & net stop WinDivert14 & sc delete WinDivert14 & net stop Monkey & sc delete Monkey");
                LocaleUtils.WriteTr("StopRemoveDrv");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Thread.Sleep(3000); // Пауза на всякий случай / Pause just in case
        }

        // Создание ярлыка Windows через PowerShell
        // Create Windows shortcut via PowerShell
        private static void CreateWindowsShortcut(string shortcutName, string targetPath, string arguments, string index = null)
        {
            string shortcutFullPath = Path.Combine(MainClass.basePath, shortcutName);

            if (File.Exists(shortcutFullPath))
            {
                return;
            }

            // PowerShell скрипт для создания ярлыка
            // PowerShell script to create shortcut
            string powershellScript = $@"
$WshShell = New-Object -ComObject WScript.Shell;
$Shortcut = $WshShell.CreateShortcut('{shortcutFullPath}');
$Shortcut.TargetPath = '{targetPath}';
$Shortcut.Arguments = '{arguments}';
$Shortcut.WorkingDirectory = '{Path.GetDirectoryName(targetPath)}';";

            // Добавляем иконку, если индекс указан
            // Add icon if index specified
            if (!string.IsNullOrEmpty(index))
            {
                powershellScript += $@"
$Shortcut.IconLocation = '{targetPath + index}';";
            }

            powershellScript += @"
$Shortcut.Save();";
            // Запускаем скрытый процесс PowerShell для каждого скрипта
            // Start hidden a PowerShell process for each script
            var psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{powershellScript}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(psi);
            Thread.Sleep(100);
        }

        // Создание всех ярлыков асинхронно
        // Create all shortcuts asynchronously
        public static async Task CreateShortcutsAsync()
        {
            try
            {
                // Создание ярлыков для различных конфигураций, может занять время
                // Creating shortcuts for various configurations, may take time
                CreateWindowsShortcut("liberty-pre STOP.lnk", selfPath, "--stop", ",1");
                CreateWindowsShortcut("liberty-pre SWITCH - ipset mode.lnk", selfPath, "--ipset", ",2");
                CreateWindowsShortcut("liberty-pre SWITCH - hidden mode.lnk", selfPath, "--hidden", ",2");
                CreateWindowsShortcut("liberty-pre SWITCH - Extended Ports.lnk", selfPath, "--extended-ports", ",2");
                // custom
                CreateWindowsShortcut("liberty-pre fake vk.lnk", selfPath, "-c default_vk_fake.cfg");
                CreateWindowsShortcut("liberty-pre extra cloudflare warp.lnk", selfPath, "-c extra-cloudflare.cfg");
                CreateWindowsShortcut("liberty-pre cringe.lnk", selfPath, "-c cringe.cfg");
                CreateWindowsShortcut("liberty-pre discord.lnk", selfPath, "-c discord.cfg");
                // from Flowseal/zapret-discord-youtube
                CreateWindowsShortcut("liberty-pre ALT.lnk", selfPath, "-c ALT.cfg");
                CreateWindowsShortcut("liberty-pre ALT1.lnk", selfPath, "-c ALT1.cfg");
                CreateWindowsShortcut("liberty-pre ALT2.lnk", selfPath, "-c ALT2.cfg");
                CreateWindowsShortcut("liberty-pre ALT3.lnk", selfPath, "-c ALT3.cfg");
                CreateWindowsShortcut("liberty-pre ALT4.lnk", selfPath, "-c ALT4.cfg");
                CreateWindowsShortcut("liberty-pre ALT5.lnk", selfPath, "-c ALT5.cfg");
                CreateWindowsShortcut("liberty-pre ALT6.lnk", selfPath, "-c ALT6.cfg");
                CreateWindowsShortcut("liberty-pre ALT7.lnk", selfPath, "-c ALT7.cfg");
                CreateWindowsShortcut("liberty-pre ALT8.lnk", selfPath, "-c ALT8.cfg");
                CreateWindowsShortcut("liberty-pre ALT9.lnk", selfPath, "-c ALT9.cfg");
                CreateWindowsShortcut("liberty-pre ALT10.lnk", selfPath, "-c ALT10.cfg");
                CreateWindowsShortcut("liberty-pre ALT11.lnk", selfPath, "-c ALT11.cfg");
                CreateWindowsShortcut("liberty-pre fake tls auto.lnk", selfPath, "-c fake_tls_auto.cfg");
                CreateWindowsShortcut("liberty-pre fake tls auto alt.lnk", selfPath, "-c fake_tls_auto_alt.cfg");
                CreateWindowsShortcut("liberty-pre fake tls auto alt2.lnk", selfPath, "-c fake_tls_auto_alt2.cfg");
                CreateWindowsShortcut("liberty-pre fake tls auto alt3.lnk", selfPath, "-c fake_tls_auto_alt3.cfg");
                CreateWindowsShortcut("liberty-pre simple fake.lnk", selfPath, "-c simple_fake.cfg");
                CreateWindowsShortcut("liberty-pre simple fake alt.lnk", selfPath, "-c simple_fake_alt.cfg");
                CreateWindowsShortcut("liberty-pre simple fake alt2.lnk", selfPath, "-c simple_fake_alt2.cfg");
            }
            catch (Exception ex)
            {
                LocaleUtils.WriteTr("CreateShortcutsFailed", ex.Message);
            }
            finally
            {
                // Сигнализируем о завершении события
                // Signal completion of the event
                ShortcutCrDone.Set();
            }
        }

        // Проверка, работает ли система под Linux
        // Check if system is running Linux
        public static bool IsLinux()
        {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }
    }
}
