using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace libertypre
{
    class MainClass
    {
        public static string basePath = AppDomain.CurrentDomain.BaseDirectory;
        private static string bindirPath = Path.Combine(basePath, "bin");
        private static string winwsExePath = Path.Combine(bindirPath, "winws.exe");
        private static bool nftables = true;

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (args.Length > 0 && (args[0] == "-h" || args[0] == "--help"))
            {
                ShowHelp();
                return;
            }
            if (args.Length > 0 && (args[0] == "-s" || args[0] == "--stop"))
            {
                if (!CringeUtils.IsLinux())
                {
                    CringeUtils.StopRemoveSevice();
                }
                return;
            }

            if (args.Length > 0 && args[0] == "-i")
            {
                nftables = false;
            }

            if (args.Length > 0 && args[0] == "-v")
            {
                ShowVersion();
                return;
            }

            if (!CheckConfigureEnvironment()) return;

            Task.Run(() => UpdCheck.CheckForUpdAsync());

            string configFile = GetConfigFile(args);
            if (!File.Exists(configFile))
            {
                LocaleUtils.WriteTr("ErrorConfigNotFound", configFile);
                return;
            }

            string arguments = ParseConfigFile(configFile);
            if (string.IsNullOrEmpty(arguments))
            {
                LocaleUtils.WriteTr("ErrorConfigEmpty");
                return;
            }

            StartWinws(arguments, configFile);
        }

        private static void ShowHelp()
        {
            LocaleUtils.WriteTr("ShowHelp");
        }

        private static void ShowVersion()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string author = "Precise";
            string projectUrl = "https://github.com/Mr-Precise/liberty-pre";

            LocaleUtils.WriteTr("ShowVersion", version, author, projectUrl);
        }

        private static bool CheckConfigureEnvironment()
        {
            if (CringeUtils.IsLinux())
            {
                LocaleUtils.WriteTr("WarningLinux");
                if (nftables)
                {
                    if (!CommandExists("nfqws"))
                    {
                        winwsExePath = Path.Combine(bindirPath, "nfqws");
                    }
                    LocaleUtils.WriteTr("InfoUsedNfqws");
                }
                else
                {
                    if (!CommandExists("tpws"))
                    {
                        winwsExePath = Path.Combine(bindirPath, "tpws");
                    }
                    LocaleUtils.WriteTr("InfoUsedIptables");
                }
            }

            if (!Directory.Exists(bindirPath))
            {
                LocaleUtils.WriteTr("ErrorBinNotFound");
                Console.ReadKey();
                return false;
            }
            if (!File.Exists(winwsExePath))
            {
                LocaleUtils.WriteTr("ErrorWinwsNotFound");
                Console.ReadKey();
                return false;
            }
            return true;
        }

        private static bool CommandExists(string command)
        {
            return CringeUtils.RunCommand("which", command).Length > 0;
        }

        private static bool IsWinwsRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }

        private static string GetConfigFile(string[] args)
        {
            if (args.Length < 2 || args[0] != "-c")
            {
                LocaleUtils.WriteTr("WarningConfigFileNotSpecified");
                if (CringeUtils.IsLinux())
                {
                    return Path.Combine(basePath, "linux.cfg");
                }
                else
                {
                    return Path.Combine(basePath, "default.cfg");
                }
            }
            return Path.Combine(basePath, args[1]);
        }

        private static string ParseConfigFile(string configFile)
        {
            return string.Join(" ", File.ReadAllLines(configFile)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("#")));
        }

        private static void StartWinws(string arguments, string configFile)
        {
            ProcessStartInfo winwsStartInfo = new ProcessStartInfo
            {
                FileName = winwsExePath,
                Arguments = arguments,
                UseShellExecute = true,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Minimized
            };

            try
            {
                string winwsProcessName = "winws";

                if (IsWinwsRunning(winwsProcessName))
                {
                    LocaleUtils.WriteTr("ErrorWinwsAldeadyRunning");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                LocaleUtils.WriteTr("DoneWinwsStarted", Path.GetFileName(configFile));
                Process.Start(winwsStartInfo);
                if (!CringeUtils.IsLinux())
                {
                    LocaleUtils.WriteTr("InfoWinwsMinimized");
                }
                UpdCheck.DoneEvent.WaitOne();
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                LocaleUtils.WriteTr("ErrorProblemStartWinws", ex.Message);
                Console.ReadKey();
            }
        }
    }
}
