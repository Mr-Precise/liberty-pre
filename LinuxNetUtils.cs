using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace libertypre
{
    public class LinuxNetUtils
    {
        // Информация для nftables таблицы
        // Info for nftables table
        private const string NftTable = "inet liberty_pre";
        private const string NftChain = "output_chain";
        private const string NftComment = "liberty-pre";
        private const int QueueNumber = 0;
        private const string NfqwsProsessName = "nfqws";

        private static string TcpPorts = "80,443,2053,2083,2087,2096,8443";
        private static string UdpPorts = "443,19294-19344,50000-50100,5054-5059,27000-27003";
        private static string ExtendedUdpPorts = ",1024-65535";

        // Проверка зависимостей для Linux
        // Check dependencies for Linux
        public static bool CheckLinuxDependencies()
        {
            string[] requiredCommands = { "nft", "pgrep", "pkill" };
            foreach (var cmd in requiredCommands)
            {
                if (!GeneralUtils.LinuxCommandExists(cmd))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    LocaleUtils.WriteTr("ErrorMissingCommand", cmd);
                    Console.ResetColor();
                    LocaleUtils.WriteTr("InfoInstallUtilityHelp");

                    return false;
                }
            }
            return true;
        }

        // Остановка процесса nfqws
        // Stop the nfqws process
        public static void StopNfqws()
        {
            try
            {
                if (GeneralUtils.IsProcessRunningBool(NfqwsProsessName))
                {
                    LocaleUtils.WriteTr("InfoStoppingNfqws");

                    // Используем прямой запуск без sudo
                    // Use a direct run without sudo
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "pkill",
                            Arguments = "-f nfqws",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    process.WaitForExit(2000);

                    // Даем время на завершение
                    // Give time for completion
                    Thread.Sleep(1000);

                    if (!GeneralUtils.IsProcessRunningBool(NfqwsProsessName))
                    {
                        LocaleUtils.WriteTr("SuccessNfqwsStopped");
                    }
                    else
                    {
                        LocaleUtils.WriteTr("WarnPkillNfqws");
                        GeneralUtils.RunLinuxSudoCommandReadToEnd("pkill -f nfqws");
                    }
                }
            }
            catch (Exception ex)
            {
                LocaleUtils.WriteTr("ErrorStopNfqws", ex.Message);
            }
        }
        
        // Проверка существования таблицы nftables
        // Check the existence of the nftables table
        private static bool TableExists()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "sudo",
                        Arguments = "nft list tables",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit(2000);

                return output.Contains(NftTable);
            }
            catch
            {
                return false;
            }
        }

        // Очистка правил nftables
        // Clear nftables rules
        public static void CleanNftablesRules()
        {
            try
            {
                if (TableExists())
                {
                    LocaleUtils.WriteTr("InfoCleaningNftRules");

                    if (GeneralUtils.RunLinuxSudoCommandReadToEnd($"nft delete table {NftTable}"))
                    {
                        LocaleUtils.WriteTr("SuccessNftRulesRemoved");
                    }
                }
            }
            catch (Exception ex)
            {
                LocaleUtils.WriteTr("ErrorCleanNftRules", ex.Message);
            }
        }

        // Настройка правил nftables
        // Setting up nftables rules
        public static bool ConfigureNftablesRules()
        {
            try
            {
                LocaleUtils.WriteTr("InfoSettingNftRules");

                // Проверяем extended ports режим
                // Check extended ports mode
                bool extendedPortsEnabled = File.Exists(Path.Combine(MainClass.basePath, "extended_ports_mode")
                );

                // Добавляем расширенныйе порты фильтрации к базовым
                // Add extended filtering ports to the basic ones
                if (extendedPortsEnabled)
                {
                    UdpPorts += ExtendedUdpPorts;
                    LocaleUtils.WriteTr("InfoNftExtendedPortsEnabled");
                }

                // Создаем nftables таблицу
                // Create a nftables table
                if (!GeneralUtils.RunLinuxSudoCommandReadToEnd($"nft add table {NftTable}"))
                {
                    LocaleUtils.WriteTr("ErrorCreateNftTable");
                    return false;
                }

                // Создаем цепочку OUTPUT
                // Передаем всю цепочку как один аргумент
                // Create an OUTPUT chain
                // Pass the entire chain as one argument
                string chainCommand = $"nft add chain {NftTable} {NftChain} '{{ type filter hook output priority 0 ; policy accept ; }}'";
                if (!GeneralUtils.RunLinuxSudoCommandReadToEnd(chainCommand))
                {
                    LocaleUtils.WriteTr("ErrorCreateNftChain");
                    return false;
                }

                // Добавляем TCP правила
                // Add TCP rules
                if (!string.IsNullOrEmpty(TcpPorts))
                {
                    string tcpCommand = $"nft add rule {NftTable} {NftChain} tcp dport \\{{ {TcpPorts} \\}} counter queue num {QueueNumber} bypass comment \\\"{NftComment}\\\"";
                    if (GeneralUtils.RunLinuxSudoCommandReadToEnd(tcpCommand))
                    {
                        LocaleUtils.WriteTr("InfoAddedTcpPortRules", TcpPorts);
                    }
                    else
                    {
                        LocaleUtils.WriteTr("WarnAddTcpRulesFailed");
                    }
                }

                // Добавляем UDP правила
                // Add UDP rules
                if (!string.IsNullOrEmpty(UdpPorts))
                {
                    string udpCommand = $"nft add rule {NftTable} {NftChain} udp dport \\{{ {UdpPorts} \\}} counter queue num {QueueNumber} bypass comment \\\"{NftComment}\\\"";
                    if (GeneralUtils.RunLinuxSudoCommandReadToEnd(udpCommand))
                    {
                        LocaleUtils.WriteTr("InfoAddedUdpPortRules", UdpPorts);
                    }
                    else
                    {
                        LocaleUtils.WriteTr("WarnAddUdpRulesFailed");
                    }
                }

                // Показываем текущие правила
                // Show current rules
                LocaleUtils.WriteTr("InfoCurrentNftCofig");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "sudo",
                        Arguments = $"nft list table {NftTable}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit(1000);
                Console.WriteLine(output);

                LocaleUtils.WriteTr("SuccessNftRulesConfigured");
                return true;
            }
            catch (Exception ex)
            {
                LocaleUtils.WriteTr("ErrorConfigureNft", ex.Message);
                return false;
            }
        }

        // Остановка всего (nfqws + nftables)
        // Stop everything (nfqws + nftables)
        public static void StopAll()
        {
            StopNfqws();
            CleanNftablesRules();
        }

        // Подготовка к запуску: остановка существующих процессов и очистка правил
        // Preparing for launch: stopping existing processes and clean rules
        public static bool PrepareForLaunch()
        {
            if (GeneralUtils.IsProcessRunningBool(NfqwsProsessName))
            {
                LocaleUtils.WriteTr("InfoNfqwsAlreadyRunning");
                StopAll();
                Thread.Sleep(2000);
            }

            return true;
        }

        // Проверка, работает ли nfqws корректно (после запуска)
        // Check if nfqws works correctly (after launch)
        public static bool VerifyNfqwsRunning()
        {
            int attempts = 0;
            while (attempts < 10) // Проверяем в течение 10 секунд / Check for 10 seconds
            {
                if (GeneralUtils.IsProcessRunningBool(NfqwsProsessName))
                {
                    LocaleUtils.WriteTr("InfoVerifyNfqwsRun");
                    return true;
                }
                Thread.Sleep(1000);
                attempts++;
            }
            LocaleUtils.WriteTr("ErrorVerifyNfqwsRun");
            return false;
        }
    }
}
