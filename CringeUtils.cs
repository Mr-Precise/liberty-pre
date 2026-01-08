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
        private static void CreateWindowsShortcut(string shortcutName, string targetPath, string arguments)
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
$Shortcut.WorkingDirectory = '{Path.GetDirectoryName(targetPath)}';
$Shortcut.Save();
";
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
                CreateWindowsShortcut("liberty-pre STOP.lnk", selfPath, "--stop");
                CreateWindowsShortcut("liberty-pre fake vk.lnk", selfPath, "-c default_vk_fake.cfg");
                CreateWindowsShortcut("liberty-pre extra cloudflare warp.lnk", selfPath, "-c extra-cloudflare.cfg");
                CreateWindowsShortcut("liberty-pre fake tls auto.lnk", selfPath, "-c default_fake_tls_mod_auto.cfg");
                CreateWindowsShortcut("liberty-pre games fake tls auto.lnk", selfPath, "-c default_games_fake_tls_mod_auto.cfg");
                CreateWindowsShortcut("liberty-pre discord.lnk", selfPath, "-c discord.cfg");
                CreateWindowsShortcut("liberty-pre games.lnk", selfPath, "-c default_games.cfg");
                CreateWindowsShortcut("liberty-pre ALT2.lnk", selfPath, "-c ALT2.cfg");
                CreateWindowsShortcut("liberty-pre games ALT2.lnk", selfPath, "-c ALT2_games.cfg");
                CreateWindowsShortcut("liberty-pre ALT3.lnk", selfPath, "-c ALT3.cfg");
                CreateWindowsShortcut("liberty-pre games ALT3.lnk", selfPath, "-c ALT3_games.cfg");
                CreateWindowsShortcut("liberty-pre ALT4.lnk", selfPath, "-c ALT4.cfg");
                CreateWindowsShortcut("liberty-pre games ALT4.lnk", selfPath, "-c ALT4_games.cfg");
                CreateWindowsShortcut("liberty-pre ALT5.lnk", selfPath, "-c ALT5.cfg");
                CreateWindowsShortcut("liberty-pre games ALT5.lnk", selfPath, "-c ALT5_games.cfg");
                CreateWindowsShortcut("liberty-pre ALT6.lnk", selfPath, "-c ALT6.cfg");
                CreateWindowsShortcut("liberty-pre games ALT6.lnk", selfPath, "-c ALT6_games.cfg");
                CreateWindowsShortcut("liberty-pre ALT9.lnk", selfPath, "-c ALT9.cfg");
                CreateWindowsShortcut("liberty-pre games ALT9.lnk", selfPath, "-c ALT9_games.cfg");
                CreateWindowsShortcut("liberty-pre ALT10.lnk", selfPath, "-c ALT10.cfg");
                CreateWindowsShortcut("liberty-pre games ALT10.lnk", selfPath, "-c ALT10_games.cfg");
                CreateWindowsShortcut("liberty-pre ALT11.lnk", selfPath, "-c ALT11.cfg");
                CreateWindowsShortcut("liberty-pre games ALT11.lnk", selfPath, "-c ALT11_games.cfg");
                CreateWindowsShortcut("liberty-pre cringe.lnk", selfPath, "-c cringe.cfg");
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
