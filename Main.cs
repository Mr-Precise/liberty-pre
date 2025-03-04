using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace libertypre
{
    class MainClass
    {
        private static string basePath = AppDomain.CurrentDomain.BaseDirectory;
        private static string bindirPath = Path.Combine(basePath, "bin");
        private static string winwsExePath = Path.Combine(bindirPath, "winws.exe");

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (args.Length > 0 && (args[0] == "-h" || args[0] == "--help"))
            {
                ShowHelp();
                return;
            }

            if (args.Length > 0 && args[0] == "-v")
            {
                ShowVersion();
                return;
            }

            if (!CheckEnvironment()) return;

            string configFile = GetConfigFile(args);
            if (!File.Exists(configFile))
            {
                Console.WriteLine($"[Ошибка]: Конфигурационный файл {configFile} не найден.");
                return;
            }

            string arguments = ParseConfigFile(configFile);
            if (string.IsNullOrEmpty(arguments))
            {
                Console.WriteLine("[Ошибка]: Конфигурационный файл пуст или содержит только комментарии.");
                return;
            }

            StartWinws(arguments, configFile);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("liberty-pre.exe -c <конфиг>.cfg");
            Console.WriteLine("Примеры запуска:");
            Console.WriteLine("  liberty-pre.exe -c discord.cfg");
            Console.WriteLine("  liberty-pre.exe -c general.cfg");
            Console.WriteLine("Примечание: по умолчанию читается файл default.cfg");
            Console.WriteLine("Аргументы:");
            Console.WriteLine("  -h, --help    Показать справку");
            Console.WriteLine("  -v            Показать версию");
        }

        private static void ShowVersion()
        {
            Console.WriteLine("liberty-pre zapret launcher 2025.3.4");
            Console.WriteLine("Автор: Precise");
            Console.WriteLine("Проект: https://github.com/Mr-Precise/liberty-pre");
        }

        private static bool CheckEnvironment()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                Console.WriteLine("[Предупреждение]: Для Linux/macOS используйте оригинальный zapret https://github.com/bol-van/zapret");
                Console.ReadKey();
                Environment.Exit(0);
            }

            if (!Directory.Exists(bindirPath))
            {
                Console.WriteLine("[Ошибка]: Каталог bin не найдена.");
                return false;
            }
            if (!File.Exists(winwsExePath))
            {
                Console.WriteLine("[Ошибка]: Файл winws.exe не найден в bin.");
                return false;
            }
            return true;
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
                Console.WriteLine("[Предупреждение]: Конфигурационный файл не указан, загружаем default.cfg");
                return Path.Combine(basePath, "default.cfg");
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
            ProcessStartInfo startInfo = new ProcessStartInfo
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
                    Console.WriteLine("[Ошибка]: Процесс winws.exe уже запущен, закройте существующий!");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                Console.WriteLine("[Готово]: winws.exe запущен с конфигурацией: " + Path.GetFileName(configFile));
                Process.Start(startInfo);
                Console.WriteLine("winws свёрнут, это окно само закроется через 3 секунды...");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Ошибка]: Проблемы при запуске winws.exe: " + ex.Message);
            }
        }
    }
}
