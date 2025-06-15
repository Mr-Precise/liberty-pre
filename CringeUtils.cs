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
                string scName = "WinDivert";
                RunCommandAsAdmin("cmd", $"/c net stop {scName} && sc delete {scName}");
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
                string selfPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                CreateWindowsShortcut("liberty-pre stop.lnk", selfPath, "--stop");
                CreateWindowsShortcut("liberty-pre fake_tls_mod_auto.lnk", selfPath, "-c default_fake_tls_mod_auto.cfg");
                CreateWindowsShortcut("liberty-pre discord.lnk", selfPath, "-c discord.cfg");
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
