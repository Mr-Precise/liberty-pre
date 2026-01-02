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
        private static string configsPath = Path.Combine(basePath, "configs");
        private static string dataPath = Path.Combine(basePath, "data");
        private static string toolDPIexe = Path.Combine(bindirPath, "winws.exe");
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
            if (!CringeUtils.IsLinux())
            {
                Task.Run(() => CringeUtils.CreateShortcutsAsync());
            }

            string configFile = GetConfigFile(args);
            if (!File.Exists(configFile))
            {
                LocaleUtils.WriteTr("ErrorConfigNotFound", configFile);
                return;
            }

            string toolarguments = ParseConfigFile(configFile);
            if (string.IsNullOrEmpty(toolarguments))
            {
                LocaleUtils.WriteTr("ErrorConfigEmpty");
                return;
            }

            if (!CringeUtils.IsLinux() && File.Exists(Path.Combine(basePath, "hidden_mode")))
            {
                if (!toolarguments.Contains("--daemon"))
                {
                    toolarguments = "--daemon " + toolarguments;
                }
                LocaleUtils.WriteTr("InfoHiddenMode");
            }

            StartUnified(toolDPIexe, toolarguments, configFile);

            if (!CringeUtils.IsLinux())
            {
                CringeUtils.ShortcutCrDone.WaitOne();
            }
            UpdCheck.DoneEvent.WaitOne();
            Thread.Sleep(3000);
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
                    if (!LinuxCommandExists("nfqws"))
                    {
                        toolDPIexe = Path.Combine(bindirPath, "nfqws");
                    }
                    LocaleUtils.WriteTr("InfoUsedNfqws");
                }
                else
                {
                    if (!LinuxCommandExists("tpws"))
                    {
                        toolDPIexe = Path.Combine(bindirPath, "tpws");
                    }
                    LocaleUtils.WriteTr("InfoUsedIptables");
                }
                if (!LinuxCommandExists("sudo"))
                {
                    LocaleUtils.WriteTr("ErrorSudoNotFound");
                }
            }

            if (!Directory.Exists(bindirPath))
            {
                LocaleUtils.WriteTr("ErrorBinNotFound");
                Console.ReadKey();
                return false;
            }

            if (!Directory.Exists(configsPath))
            {
                LocaleUtils.WriteTr("ErrorConfDirNotFound");
                Directory.CreateDirectory(configsPath);
                return false;
            }

            if (!Directory.Exists(dataPath))
            {
                LocaleUtils.WriteTr("ErrorDataDirNotFound");
                Directory.CreateDirectory(dataPath);
                return false;
            }

            if (!CringeUtils.IsLinux())
            {
                if (!File.Exists(toolDPIexe))
                {
                    LocaleUtils.WriteTr("ErrorWinwsNotFound");
                    Console.ReadKey();
                    return false;
                }
            }
            return true;
        }

        private static bool LinuxCommandExists(string command)
        {
            return CringeUtils.RunCommand("which", command).Length > 0;
        }

        private static bool IsProcessRunning(string processName)
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
                    return Path.Combine(configsPath, "linux.cfg");
                }
                else
                {
                    return Path.Combine(configsPath, "default.cfg");
                }
            }
            if (!File.Exists(Path.Combine(configsPath, args[1])))
            {
                LocaleUtils.WriteTr("ErrorConfigFileNotFound", args[1]);
                Console.ReadKey();
            }
            return Path.Combine(configsPath, args[1]);
        }

        private static string ParseConfigFile(string configFile)
        {
            return string.Join(" ", File.ReadAllLines(configFile)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("#")));
        }

        private static void StartUnified(string toolprogram, string toolarguments, string configFile)
        {
            string lastToolProgram = toolprogram;
            string lastToolArguments = toolarguments;
            bool shellexToggle = true;
            string processName = "winws";

            if (CringeUtils.IsLinux())
            {
                processName = "nfqws";
                shellexToggle = false;
                lastToolProgram = "sudo";
                lastToolArguments = $"{toolprogram} {toolarguments}";
            }

            if (IsProcessRunning(processName))
            {
                LocaleUtils.WriteTr("WarningAldeadyRunning");
                CringeUtils.StopRemoveSevice();
            }

            var UniStartInfo = new ProcessStartInfo();
            UniStartInfo.FileName = lastToolProgram;
            UniStartInfo.Arguments = lastToolArguments;
            UniStartInfo.UseShellExecute = shellexToggle;
            UniStartInfo.WorkingDirectory = dataPath;

            if (!CringeUtils.IsLinux())
            {
                UniStartInfo.Verb = "runas";
                UniStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            }

            try
            {
                LocaleUtils.WriteTr("DoneWinwsStarted", Path.GetFileName(configFile));
                if (CringeUtils.IsLinux())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    LocaleUtils.WriteTr("WarningLinuxRootPassword");
                    Console.ResetColor();

                    Process uni = Process.Start(UniStartInfo);
                    uni.WaitForExit();
                }
                else
                {
                    LocaleUtils.WriteTr("InfoWinwsMinimized");
                    Process.Start(UniStartInfo);
                }
            }
            catch (Exception ex)
            {
                LocaleUtils.WriteTr("ErrorProblemStartWinws", ex.Message);
                Console.ReadKey();
            }
        }
    }
}
