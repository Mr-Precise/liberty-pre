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
        public static ManualResetEvent ShortcutCrDone = new ManualResetEvent(false);

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

        private static void RunCommandAsAdmin(string command, string args)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                UseShellExecute = true,
                Verb = "runas",
                CreateNoWindow = true
            };
            Process.Start(psi);
        }

        public static void StopRemoveSevice()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("winws");
                foreach (Process proc in processes)
                {
                    proc.Kill();
                    proc.WaitForExit(1000);
                }

                RunCommandAsAdmin("cmd", $"/c net stop WinDivert & sc delete WinDivert & net stop WinDivert14 & sc delete WinDivert14 & net stop Monkey & sc delete Monkey");
                LocaleUtils.WriteTr("StopRemoveDrv");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Thread.Sleep(3000);
        }

        private static void CreateWindowsShortcut(string shortcutName, string targetPath, string arguments)
        {
            string shortcutFullPath = Path.Combine(MainClass.basePath, shortcutName);

            if (File.Exists(shortcutFullPath))
            {
                return;
            }

            string powershellScript = $@"
$WshShell = New-Object -ComObject WScript.Shell;
$Shortcut = $WshShell.CreateShortcut('{shortcutFullPath}');
$Shortcut.TargetPath = '{targetPath}';
$Shortcut.Arguments = '{arguments}';
$Shortcut.WorkingDirectory = '{Path.GetDirectoryName(targetPath)}';
$Shortcut.Save();
";

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

        public static async Task CreateShortcutsAsync()
        {
            try
            {
                string selfPath = Assembly.GetExecutingAssembly().Location;

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
                ShortcutCrDone.Set();
            }
        }

        public static bool IsLinux()
        {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }
    }
}
