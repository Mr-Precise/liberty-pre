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
        // Базовые пути приложения
        // Application base paths
        public static string basePath = AppDomain.CurrentDomain.BaseDirectory;
        private static string bindirPath = Path.Combine(basePath, "bin");
        private static string configsPath = Path.Combine(basePath, "configs");
        public static string dataPath = Path.Combine(basePath, "data");
        private static string toolDPIexe = Path.Combine(bindirPath, "winws.exe");
        private static bool nftables = true;

        public static void Main(string[] args)
        {
            // Устанавливаем кодировку консоли для поддержки UTF-8
            // Set console encoding for UTF-8 support
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Как ни странно, обработка аргументов командной строки
            // Command line arguments processing
            if (args.Length > 0 && (args[0] == "-h" || args[0] == "--help"))
            {
                ShowHelp();
                return;
            }
            if (args.Length > 0 && (args[0] == "-s" || args[0] == "--stop"))
            {
                // Остановка служб и драйверов (только на Windows)
                // Stop services and drivers (Windows only)
                if (!CringeUtils.IsLinux())
                {
                    CringeUtils.StopRemoveSevice();
                }
                return;
            }

            // Флаг отключения nftables (использование iptables, позже будет удалено)
            // Disable nftables flag (use iptables, will be deleted later)
            if (args.Length > 0 && args[0] == "-i")
            {
                nftables = false;
            }

            // Показываем версию
            // Show version
            if (args.Length > 0 && args[0] == "-v")
            {
                ShowVersion();
                return;
            }

            // Переключение режима ipset файла
            // Ipset file mode switch
            if (args.Length > 0 && args[0] == "--ipset")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                IpsetSwitchUtils.IpsetSwitchNext();
                Console.ForegroundColor = ConsoleColor.Green;
                GetIpsetStatus();
                Console.ResetColor();
                Thread.Sleep(3000);
                return;
            }

            // Проверка окружения перед запуском
            // Environment check before startup
            if (!CheckConfigureEnvironment()) return;

            // Запуск фоновых задач
            // Start background tasks
            Task.Run(() => UpdCheck.CheckForUpdAsync());
            if (!CringeUtils.IsLinux())
            {
                Task.Run(() => CringeUtils.CreateShortcutsAsync());
            }

            // Определение файла конфигурации
            // Determine configuration file
            string configFile = GetConfigFile(args);
            if (!File.Exists(configFile))
            {
                LocaleUtils.WriteTr("ErrorConfigNotFound", configFile);
                return;
            }

            // Парсинг конфигурационного файла в аргументы
            // Parse configuration file to arguments
            string toolarguments = ParseConfigFile(configFile);
            if (string.IsNullOrEmpty(toolarguments))
            {
                LocaleUtils.WriteTr("ErrorConfigEmpty");
                return;
            }

            // Добавление флага --daemon для скрытого режима (Windows)
            // Add --daemon flag for hidden mode (Windows)
            if (!CringeUtils.IsLinux() && File.Exists(Path.Combine(basePath, "hidden_mode")))
            {
                if (!toolarguments.Contains("--daemon"))
                {
                    toolarguments = "--daemon " + toolarguments;
                }
                LocaleUtils.WriteTr("InfoHiddenMode");
            }

            // Выводим статус использования ipset листа
            // Displaying the status of using the ipset list
            GetIpsetStatus();

            // Запуск основной утилиты (winws / nfqws)
            // Start main utility (winws / nfqws)
            StartUnified(toolDPIexe, toolarguments, configFile);

            // Ожидание завершения фоновых задач
            // Wait for background tasks completion
            if (!CringeUtils.IsLinux())
            {
                CringeUtils.ShortcutCrDone.WaitOne();
            }
            UpdCheck.DoneEvent.WaitOne();
            Thread.Sleep(3000); // Финализация / Finalization
        }

        // Вывод справки
        // Display usage help
        private static void ShowHelp()
        {
            LocaleUtils.WriteTr("ShowHelp");
        }

        // Вывод информации о версии и авторе
        // Display version and author information 
        private static void ShowVersion()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string author = "Precise";
            string projectUrl = "https://github.com/Mr-Precise/liberty-pre";

            LocaleUtils.WriteTr("ShowVersion", version, author, projectUrl);
        }

        // Проверка и настройка окружения
        // Check and configure environment
        private static bool CheckConfigureEnvironment()
        {
            // Обработка Linux-специфичных настроек
            // Handle Linux-specific settings
            if (CringeUtils.IsLinux())
            {
                LocaleUtils.WriteTr("WarningLinux");
                if (nftables)
                {
                    // Использование nfqws (nftables)
                    // Use nfqws (nftables)
                    if (!LinuxCommandExists("nfqws"))
                    {
                        toolDPIexe = Path.Combine(bindirPath, "nfqws");
                    }
                    LocaleUtils.WriteTr("InfoUsedNfqws");
                }
                else
                {
                    // Использование tpws (iptables)
                    // Use tpws (iptables)
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

            // Проверка существования каталога bin
            // Check bin directory existence
            if (!Directory.Exists(bindirPath))
            {
                LocaleUtils.WriteTr("ErrorBinNotFound");
                Console.ReadKey();
                return false;
            }

            // Проверка / создание каталога configs
            // Check / create configs directory
            if (!Directory.Exists(configsPath))
            {
                LocaleUtils.WriteTr("ErrorConfDirNotFound");
                Directory.CreateDirectory(configsPath);
                return false;
            }

            // Проверка / создание каталога data
            // Check / create data directory
            if (!Directory.Exists(dataPath))
            {
                LocaleUtils.WriteTr("ErrorDataDirNotFound");
                Directory.CreateDirectory(dataPath);
                return false;
            }

            // Проверка существования winws.exe (Windows)
            // Check winws.exe existence (Windows)
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

        // Проверка наличия команды в Linux
        // Check if command exists in Linux
        private static bool LinuxCommandExists(string command)
        {
            return CringeUtils.RunCommand("which", command).Length > 0;
        }

        // Проверка запущен ли процесс
        // Check if process is running
        private static bool IsProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }

        // Метод для вывода статуса с локализацей использования ipset листа
        // Method for displaying status with localization using ipset list
        private static void GetIpsetStatus()
        {
            string statusKey = IpsetSwitchUtils.IpsetSwitchStatus() ? "IpsetStatusEnabled" : "IpsetStatusStub";
            LocaleUtils.WriteTr("InfoUseIpsetFile", LocaleUtils.GetStrTr(statusKey));
        }

        // Определение файла конфигурации на основе аргументов
        // Determine configuration file based on arguments
        private static string GetConfigFile(string[] args)
        {
            if (args.Length < 2 || args[0] != "-c")
            {
                LocaleUtils.WriteTr("WarningConfigFileNotSpecified");
                if (CringeUtils.IsLinux())
                {
                    // Отдельная конфигурация для Linux
                    // Separate configuration for Linux
                    return Path.Combine(configsPath, "linux.cfg");
                }
                else
                {
                    // default.cfg если ничего не выбрано
                    // default.cfg if nothing is selected
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

        // Парсинг конфигурационного файла в строку аргументов
        // Parse configuration file to arguments into one string
        private static string ParseConfigFile(string configFile)
        {
            return string.Join(" ", File.ReadAllLines(configFile)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("#")));
        }

        // Унифицированный запуск утилиты
        // Unified utility startup
        private static void StartUnified(string toolprogram, string toolarguments, string configFile)
        {
            string lastToolProgram = toolprogram;
            string lastToolArguments = toolarguments;
            bool shellexToggle = true;
            string processName = "winws";

            // Настройка для Linux
            // Setup for Linux
            if (CringeUtils.IsLinux())
            {
                processName = "nfqws";
                shellexToggle = false;
                lastToolProgram = "sudo"; // Требуются права root / Requires root 
                lastToolArguments = $"{toolprogram} {toolarguments}";
            }

            // Проверка, не запущена ли утилита уже
            // Check if utility is already running
            if (IsProcessRunning(processName))
            {
                LocaleUtils.WriteTr("WarningAldeadyRunning");
                CringeUtils.StopRemoveSevice();
            }

            // Настройка параметров запуска процесса
            // Configure process startup parameters
            var UniStartInfo = new ProcessStartInfo();
            UniStartInfo.FileName = lastToolProgram;
            UniStartInfo.Arguments = lastToolArguments;
            UniStartInfo.UseShellExecute = shellexToggle;
            UniStartInfo.WorkingDirectory = dataPath;

            // Настройки для Windows (права администратора)
            // Settings for Windows (administrator rights)
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

                    // Синхронный запуск с ожиданием (Linux)
                    // Synchronous startup with waiting (Linux)
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
