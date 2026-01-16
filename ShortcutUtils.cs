using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace libertypre
{
    public class ShortcutUtils
    {
        // Событие завершения создания ярлыков
        // Shortcut creation completion event
        public static ManualResetEvent ShortcutCrDone = new ManualResetEvent(false);
        private static string selfPath = Assembly.GetExecutingAssembly().Location;


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
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"\"{powershellScript}\"",
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
    }
}
