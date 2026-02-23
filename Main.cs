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

        // Константы для замены Extended Ports (GameFilter)
        // Extended Ports (GameFilter) replacement constants
        private const string ExtendedPortsEnabled = "1024-65535";
        private const string ExtendedPortsDisabled = "12";

        public static void Main(string[] args)
        {
            // Устанавливаем кодировку консоли для поддержки UTF-8
            // Set console encoding for UTF-8 support
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string configFileArg = null;

            // Как ни странно, обработка аргументов командной строки
            // Command line arguments processing
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg == "-h" || arg == "--help")
                {
                    ShowHelp();
                    return;
                }
                else if (arg == "-s" || arg == "--stop")
                {
                    if (!GeneralUtils.IsLinux())
                    {
                        // Остановка служб и драйверов (Windows)
                        // Stop services and drivers (Windows)
                        GeneralUtils.StopRemoveWidowsSevice();
                    }
                    else
                    {
                        // Остановка процессов и очистка правил (Linux)
                        // Stop processes and clean rules (Linux)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        LinuxNetUtils.StopAll();
                        Console.ForegroundColor = ConsoleColor.Green;
                        LocaleUtils.WriteTr("InfoAllStopAndCleanRules");
                        Console.ResetColor();
                        Thread.Sleep(2000);
                        return;
                    }
                    return;
                }
                else if (arg == "-v")
                {
                    // Показываем версию
                    // Show version
                    ShowVersion();
                    return;
                }
                else if (arg == "--extended-ports")
                {
                    // Обработка аргумента --extended-ports (переключатель)
                    // Handle --extended-ports argument (toggle switch)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    ExtendedPortsFilterUtils.ToggleExtendedPortsMode();
                    Console.ForegroundColor = ConsoleColor.Green;
                    ShowExtendedPortsModeStatus();
                    Console.ResetColor();
                    Thread.Sleep(3000);
                    return;
                }
                else if (arg == "--ipset")
                {
                    // Переключение режима ipset файла
                    // Ipset file mode switch
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    IpsetSwitchUtils.IpsetSwitchNext();
                    Console.ForegroundColor = ConsoleColor.Green;
                    ShowIpsetStatus();
                    Console.ResetColor();
                    Thread.Sleep(3000);
                    return;
                }
                else if (arg == "--hidden")
                {
                    // Обработка аргумента --hidden (переключатель)
                    // Handle --hidden argument (toggle switch)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    HiddenModeSwchUtils.ToggleHiddenMode();
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (HiddenModeSwchUtils.GetHiddenModeStatus())
                    {
                        LocaleUtils.WriteTr("InfoHiddenMode");
                    }
                    Console.ResetColor();
                    Thread.Sleep(3000);
                    return;
                }
                else if (arg == "-i")
                {
                    // Отключение nftables
                    // Disable nftables
                    nftables = false;
                }
                else if (arg == "-c" && i + 1 < args.Length)
                {
                    // Следующий аргумент это имя файла конфигурации
                    // Next argument is the name of the configuration file
                    configFileArg = args[i + 1];
                    // Пропускаем следующий аргумент, т.к. он уже обработан
                    // Skip the next argument because it has already been processed
                    i++;
                }
                else if (arg.StartsWith("-"))
                {
                    // Неизвестный аргумент
                    // Unknown argument
                    LocaleUtils.WriteTr("WarningCmdUnknownArgument", arg);
                    ShowHelp();
                    return;
                }
            }

            // Проверка окружения перед запуском
            // Environment check before startup
            if (!CheckConfigureEnvironment()) return;

            // Запуск фоновых задач
            // Start background tasks
            Task.Run(() => UpdCheck.CheckForUpdAsync());
            if (!GeneralUtils.IsLinux())
            {
                Task.Run(() => ShortcutUtils.CreateShortcutsAsync());
            }

            // Определение файла конфигурации
            // Determine configuration file
            string configFile = GetConfigFile(configFileArg);
            if (!File.Exists(configFile))
            {
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

            // Выводим статус расширенного режима портов
            // Displaying the status of Extended Ports mode
            ShowExtendedPortsModeStatus();

            // Добавление флага --daemon для скрытого режима (Windows)
            // Add --daemon flag for hidden mode (Windows)
            if (!GeneralUtils.IsLinux() && HiddenModeSwchUtils.GetHiddenModeStatus())
            {
                if (!toolarguments.Contains("--daemon"))
                {
                    toolarguments = "--daemon " + toolarguments;
                }
                LocaleUtils.WriteTr("InfoHiddenMode");
            }

            // Выводим статус использования ipset листа
            // Displaying the status of using the ipset list
            ShowIpsetStatus();

            // Запуск основной утилиты (winws / nfqws)
            // Start main utility (winws / nfqws)
            StartUnified(toolDPIexe, toolarguments, configFile);

            // Ожидание завершения фоновых задач
            // Wait for background tasks completion
            if (!GeneralUtils.IsLinux())
            {
                ShortcutUtils.ShortcutCrDone.WaitOne();
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
            if (GeneralUtils.IsLinux())
            {
                LocaleUtils.WriteTr("WarningLinux");

                if (!GeneralUtils.UnixCommandExists("sudo"))
                {
                    LocaleUtils.WriteTr("ErrorSudoNotFound");
                }

                // Проверка зависимостей Linux
                // Check Linux dependencies
                if (!LinuxNetUtils.CheckLinuxDependencies())
                {
                    Console.ReadKey();
                    return false;
                }

                if (nftables)
                {
                    // Использование nfqws (nftables)
                    // Use nfqws (nftables)
                    if (!GeneralUtils.UnixCommandExists("nfqws"))
                    {
                        toolDPIexe = Path.Combine(bindirPath, "nfqws");
                    }
                    LocaleUtils.WriteTr("InfoUsedNfqws");
                }
                else
                {
                    // Использование tpws
                    // Use tpws
                    if (!GeneralUtils.UnixCommandExists("tpws"))
                    {
                        toolDPIexe = Path.Combine(bindirPath, "tpws");
                    }
                    LocaleUtils.WriteTr("InfoUsedTpwsLinux");
                }

                // Подготовка к запуску
                // Preparing for launch
                if (!LinuxNetUtils.PrepareForLaunch())
                {
                    return false;
                }
            }
            // Обработка macOS-специфичных настроек
            // Handle macOS-specific settings
            if (GeneralUtils.IsMacOS())
            {
                LocaleUtils.WriteTr("WarningMacOS");

                // Использование tpws
                // Use tpws
                toolDPIexe = Path.Combine(bindirPath, "tpws");
                LocaleUtils.WriteTr("InfoUsedTpwsMacOS");
            }

            // Проверка и включение TCP timestamps, если он отключен (Windows)
            // Check and enable TCP timestamps if it is disabled (Windows)
            if (!GeneralUtils.IsLinux() && !GeneralUtils.IsMacOS())
            {
                GeneralUtils.CheckAndEnableTcpTimestamps();
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
            if (!GeneralUtils.IsLinux())
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

        // Метод для вывода статуса (с локализацей) использования ipset листа
        // Method for displaying status (with localization) using ipset list
        private static void ShowIpsetStatus()
        {
            string statusKey = IpsetSwitchUtils.IpsetSwitchStatus() ? "IpsetStatusEnabled" : "IpsetStatusStub";
            LocaleUtils.WriteTr("InfoUseIpsetFile", LocaleUtils.GetStrTr(statusKey));
        }

        // Метод для вывода статуса (с локализацей) использования расширенного режима портов
        // Method for displaying status (with localization) using extended ports mode
        private static void ShowExtendedPortsModeStatus()
        {
            string extendedPortsStr = ExtendedPortsFilterUtils.GetExtendedPortsModeStatus() ? "ExtendedPortsModeEnabled" : "ExtendedPortsModeDisabled";
            LocaleUtils.WriteTr("InfoExtendedPortsModeFilter", LocaleUtils.GetStrTr(extendedPortsStr));
        }

        // Определение файла конфигурации на основе аргументов
        // Determine configuration file based on arguments
        private static string GetConfigFile(string cfgName = null)
        {
            if (string.IsNullOrEmpty(cfgName))
            {
                LocaleUtils.WriteTr("WarningConfigFileNotSpecified");
                if (GeneralUtils.IsLinux())
                {
                    // Отдельная конфигурация для Linux
                    // Separate configuration for Linux
                    return Path.Combine(configsPath, "linux.cfg");
                }
                else if (GeneralUtils.IsMacOS())
                {
                    // Отдельная конфигурация для macOS
                    // Separate configuration for macOS
                    return Path.Combine(configsPath, "macos.cfg");
                }
                else
                {
                    // default.cfg если ничего не выбрано
                    // default.cfg if nothing is selected
                    return Path.Combine(configsPath, "default.cfg");
                }
            }
            string fullPath = Path.Combine(configsPath, cfgName);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            // Пробуем добавить расширение .cfg если его нет
            // Try adding .cfg extension if it's not there
            if (string.IsNullOrEmpty(Path.GetExtension(cfgName)))
            {
                fullPath = Path.Combine(configsPath, cfgName + ".cfg");
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            if (!File.Exists(fullPath))
            {
                LocaleUtils.WriteTr("ErrorConfigFileNotFound", cfgName);

                // Выводим список доступных конфигурационных файлов
                // Display a list of available configuration files
                LocaleUtils.WriteTr("InfoAvailableConfigFiles");
                foreach (string file in Directory.GetFiles(configsPath, "*.cfg"))
                {
                    Console.WriteLine($"  {Path.GetFileName(file)}");
                }
                Console.ReadKey();
            }
            return fullPath;
        }

        // Парсинг конфигурационного файла в строку аргументов
        // Parse configuration file to arguments into one string
        private static string ParseConfigFile(string configFile)
        {
            // Определяем, включен ли расширенный режим портов
            // Determine if the extended ports mode is enabled
            bool epMode = ExtendedPortsFilterUtils.GetExtendedPortsModeStatus();
            string replacement = epMode ? ExtendedPortsEnabled : ExtendedPortsDisabled;

            return string.Join(" ", File.ReadAllLines(configFile)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                .Select(line => line.Replace("@ExtendedPorts@", replacement))
                .Select(line => line.Replace("^!", "!")));
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
            if (GeneralUtils.IsLinux())
            {
                processName = "nfqws";
                shellexToggle = false;
                lastToolProgram = "sudo"; // Требуются права root / Requires root 
                lastToolArguments = $"{toolprogram} {toolarguments}";
            }

            // Проверка, не запущена ли утилита уже
            // Check if utility is already running
            if (GeneralUtils.IsProcessRunningBool(processName))
            {
                LocaleUtils.WriteTr("WarningAldeadyRunning");
                if (GeneralUtils.IsLinux())
                {
                    LinuxNetUtils.StopAll();
                }
                else
                {
                    GeneralUtils.StopRemoveWidowsSevice();
                }
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
            if (!GeneralUtils.IsLinux())
            {
                UniStartInfo.Verb = "runas";
                UniStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            }

            try
            {
                LocaleUtils.WriteTr("DoneWinwsStarted", Path.GetFileName(configFile));
                if (GeneralUtils.IsLinux())
                {
                    Thread.Sleep(2000);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    LocaleUtils.WriteTr("WarningLinuxRootPassword");
                    Console.ResetColor();

                    // Синхронный запуск с ожиданием sudo (Linux)
                    // Synchronous startup with waiting sudo (Linux)
                    Process uni = Process.Start(UniStartInfo);
                    uni.WaitForExit(); // TODO

                    // На всякий случай даем время процессу запуститься
                    // Just in case, we give the process time to start normally
                    Thread.Sleep(3000);

                    // Проверяем, запустился ли nfqws
                    // Check if nfqws has started
                    if (nftables && !LinuxNetUtils.VerifyNfqwsRunning())
                    {
                        LocaleUtils.WriteTr("ErrorFirstVerifyNfqwsRun");
                        LinuxNetUtils.StopAll();
                        return;
                    }

                    // Настраиваем nftables правила
                    // Set up nftables rules
                    if (nftables && LinuxNetUtils.ConfigureNftablesRules())
                    {
                        LocaleUtils.WriteTr("InfoSuccessRunWithRules");

                        // Ожидаем / перехватываем Ctrl+C
                        // Wait for / catching Ctrl+C
                        ManualResetEvent exitEvent = new ManualResetEvent(false);
                        Console.CancelKeyPress += (sender, e) =>
                        {
                            e.Cancel = true;
                            exitEvent.Set();
                        };

                        exitEvent.WaitOne();

                        // Очищаем правила перед выходом
                        // Clear the rules before exiting
                        LinuxNetUtils.StopAll();
                        LocaleUtils.WriteTr("InfoCleanupComletedExit");
                    }
                    else
                    {
                        LocaleUtils.WriteTr("ErrorConfigureNftExit");
                        LinuxNetUtils.StopAll();
                    }
                }
                else
                {
                    // Запускаем winws (Windows)
                    // Run winws (Windows)
                    LocaleUtils.WriteTr("InfoWinwsMinimized");
                    Process.Start(UniStartInfo);
                }
            }
            catch (Exception ex)
            {
                // Странная проблема при запуске winws или nfqws
                // Strange problems when starting winws или nfqws
                LocaleUtils.WriteTr("ErrorProblemStartToolDPI", ex.Message);
                if (GeneralUtils.IsLinux())
                {
                    LinuxNetUtils.StopAll();
                }
                Console.ReadKey();
            }
        }
    }
}
