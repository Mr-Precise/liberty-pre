using System;
using System.Collections.Generic;
using System.Globalization;

namespace libertypre
{
    public class LocaleUtils
    {
        // Словарь для хранения переводов
        // Dictionary for storing translations
        private static Dictionary<string, string> _translations;

        // Статический конструктор для инициализации словаря переводов
        // Static constructor for initializing translation dictionary
        static LocaleUtils()
        {
            _translations = new Dictionary<string, string>();

            _translations.Add("en_ErrorConfigFileNotFound", "[Error]: Configuration file {0} not found!");
            _translations.Add("ru_ErrorConfigFileNotFound", "[Ошибка]: Файл конфигурации {0} не найден!");

            _translations.Add("en_InfoAvailableConfigFiles", "[Info]: List of available configuration files:");
            _translations.Add("ru_InfoAvailableConfigFiles", "[Информация]: Список доступных конфигурационных файлов:");

            _translations.Add("en_ErrorConfigEmpty", "[Error]: Configuration file is empty or contains only comments.");
            _translations.Add("ru_ErrorConfigEmpty", "[Ошибка]: Конфигурационный файл пуст или содержит только комментарии.");

            _translations.Add("en_WarningLinux", "[Warning]: Linux version is used. Recommended for advanced users.");
            _translations.Add("ru_WarningLinux", "[Предупреждение]: Используется версия для Linux. Рекомендуется для продвинутых пользователей.");

            _translations.Add("en_WarningMacOS", "[Warning]: macOS version not supported. Recommended for advanced users.");
            _translations.Add("ru_WarningMacOS", "[Предупреждение]: Версия для macOS не поддерживается. Рекомендуется для продвинутых пользователей.");

            _translations.Add("en_WarningCmdUnknownArgument", "[Warning]: Unknown argument: {0}");
            _translations.Add("ru_WarningCmdUnknownArgument", "[Предупреждение]: Неизвестный аргумент: {0}");

            _translations.Add("en_WarningLinuxRootPassword", "[Linux]: (root access) Enter sudo password: ");
            _translations.Add("ru_WarningLinuxRootPassword", "[Linux]: (root доступ) Введите пароль sudo: ");

            _translations.Add("en_ErrorSudoNotFound", "[Error]: Requires sudo to work properly.");
            _translations.Add("ru_ErrorSudoNotFound", "[Ошибка]: Для правильной работы требуется sudo.");

            _translations.Add("en_ErrorBinNotFound", "[Error]: The bin directory not found.");
            _translations.Add("ru_ErrorBinNotFound", "[Ошибка]: Каталог bin не найден.");

            _translations.Add("en_ErrorConfDirNotFound", "[Error]: The configs directory not found.");
            _translations.Add("ru_ErrorConfDirNotFound", "[Ошибка]: Каталог configs не найден.");

            _translations.Add("en_DataDirNotFound", "[Error]: The data directory not found.");
            _translations.Add("ru_DataDirNotFound", "[Ошибка]: Каталог data не найден.");

            _translations.Add("en_ErrorWinwsNotFound", "[Error]: File winws.exe not found in bin.");
            _translations.Add("ru_ErrorWinwsNotFound", "[Ошибка]: Файл winws.exe не найден в bin.");

            _translations.Add("en_WarningConfigFileNotSpecified", "[Warning]: Configuration file not specified, loading default.");
            _translations.Add("ru_WarningConfigFileNotSpecified", "[Предупреждение]: Конфигурационный файл не указан, загружаем по умолчанию.");

            _translations.Add("en_InfoUsedNfqws", "[Info]: Linux / Used nfqws (nftables).");
            _translations.Add("ru_InfoUsedNfqws", "[Информация]: Linux / Используется nfqws (nftables).");

            _translations.Add("en_InfoHiddenMode", "[Info]: The hidden / invisible mode of operation (daemon) is used.");
            _translations.Add("ru_InfoHiddenMode", "[Информация]: Используется скрытый / невидимый режим работы (daemon).");

            _translations.Add("en_InfoEnablingHiddenMode", "[Info]: Enabling hidden mode... (the hidden_mode file is created).");
            _translations.Add("ru_InfoEnablingHiddenMode", "[Информация]: Включение скрытого режима... (создание файла hidden_mode).");

            _translations.Add("en_InfoDisablingHiddenMode", "[Info]: Disabling hidden mode... (deleting the hidden_mode file).");
            _translations.Add("ru_InfoDisablingHiddenMode", "[Информация]: Выключние скрытого режима... (удаление файла hidden_mode).");

            _translations.Add("en_InfoUsedTpwsLinux", "[Info]: Linux / Used tpws, limited in functionality.");
            _translations.Add("ru_InfoUsedTpwsLinux", "[Информация]: Linux / Используется tpws, ограниченная по функционалу.");

            _translations.Add("en_InfoUsedTpwsMacOS", "[Info]: macOS / Used tpws, limited in functionality.");
            _translations.Add("ru_InfoUsedTpwsMacOS", "[Информация]: macOS / Используется tpws, ограниченная по функционалу.");

            _translations.Add("en_WarningAldeadyRunning", "[Warning]: The process has already been running, is being restarting!");
            _translations.Add("ru_WarningAldeadyRunning", "[Предупреждение]: Процесс уже был запущен, выполняется перезапуск!");

            _translations.Add("en_DoneWinwsStarted", "[Done]: Started with configuration: {0}");
            _translations.Add("ru_DoneWinwsStarted", "[Готово]: Запущено с конфигурацией: {0}");

            _translations.Add("en_InfoWinwsMinimized", "[Info]: winws is minimized, this window will close itself in 3 seconds...");
            _translations.Add("ru_InfoWinwsMinimized", "[Информация]: winws теперь свёрнут, это окно само закроется через 3 секунды...");

            _translations.Add("en_ErrorProblemStartToolDPI", "[Error]: Problems starting winws.exe or nfqws: {0}");
            _translations.Add("ru_ErrorProblemStartToolDPI", "[Ошибка]: Проблемы при запуске winws.exe или nfqws: {0}");

            _translations.Add("en_ShowVersion",
                "liberty-pre zapret launcher {0}\n" +
                "Author:  {1}\n" +
                "Project: {2}");
            _translations.Add("ru_ShowVersion",
                "liberty-pre zapret лаунчер {0}\n" +
                "Автор:  {1}\n" +
                "Проект: {2}");

            _translations.Add("en_ShowHelp",
                "liberty-pre.exe -c <config>.cfg\n" +
                "Launch examples:\n" +
                "  liberty-pre.exe -c discord.cfg\n" +
                "  liberty-pre.exe -c general.cfg\n" +
                "Note: by default file used is default.cfg\n" +
                "Arguments:\n" +
                "  -h, --help       Show help\n" +
                "  -c               Configuration file name (in the configs directory)\n" +
                "  -i               Use tpws instead of nfqws (nftables)\n" +
                "  -s, --stop       Stop winws / nfqws and delete WinDivert Service + clean rules\n" +
                "  --ipset          Switch ipset file mode (full or stub)\n" +
                "  --extended-ports Switch extended ports mode (port filtering 1024-65535)\n" +
                "  --hidden         Switch hidden / invisible  mode of operation (daemon)\n" +
                "  -v               Show version");
            _translations.Add("ru_ShowHelp",
                "liberty-pre.exe -c <конфиг>.cfg\n" +
                "Примеры запуска:\n" +
                "  liberty-pre.exe -c discord.cfg\n" +
                "  liberty-pre.exe -c general.cfg\n" +
                "Примечание: по умолчанию читается файл default.cfg\n" +
                "Аргументы:\n" +
                "  -h, --help       Показать справку\n" +
                "  -c               Имя файла конфигурации (в каталоге configs)\n" +
                "  -i               Использовать tpws вместо nfqws (nftables)\n" +
                "  -s, --stop       Остановить winws / nfqws и удалить WinDivert сервис + очистка правил\n" +
                "  --ipset          Переключатель режима работы ipset (полный или заглушка)\n" +
                "  --extended-ports Переключатель расширенного режима портов (фильтрация портов 1024-65535)\n" +
                "  --hidden         Переключатель скрытого / невидимого режима работы (daemon)\n" +
                "  -v               Показать версию");

            _translations.Add("en_CreateShortcutsFailed", "[Error]: Error while creating shortcuts (.lnk): {0}");
            _translations.Add("ru_CreateShortcutsFailed", "[Ошибка]: Ошибка при создании ярлыков (.lnk): {0}");

            _translations.Add("en_UpdCheckFailed", "[Error]: Error when checking updates: {0}");
            _translations.Add("ru_UpdCheckFailed", "[Ошибка]: Ошибка при проверке обновлений: {0}");

            _translations.Add("en_UpdCheckLocal", "Local version: ");
            _translations.Add("ru_UpdCheckLocal", "Локальная версия: ");

            _translations.Add("en_UpdCheckRemote", ", Remote version (GitHub): ");
            _translations.Add("ru_UpdCheckRemote", ", Удаленная версия (GinHub): ");

            _translations.Add("en_UpdCheckLatest", "[UPD]: You have the latest version.");
            _translations.Add("ru_UpdCheckLatest", "[UPD]: У вас актуальная версия.");

            _translations.Add("en_StopRemoveDrv", "[Done]: winws is stopped + WinDivert unload.");
            _translations.Add("ru_StopRemoveDrv", "[Готово]: winws завершен + выгружен WinDivert.");

            _translations.Add("en_UpdateSkip", "[UPD]: Update notifications disabled by user.");
            _translations.Add("ru_UpdateSkip", "[UPD]: Уведомления об обновлениях отключены пользователем.");

            _translations.Add("en_UpdateAvailableDialog", "Update Available");
            _translations.Add("ru_UpdateAvailableDialog", "Доступно обновление");

            _translations.Add("en_UpdateAvailableDialogNew",
                "new version available\n\n" +
                "[Yes] - Open update page in browser\n" +
                "[No] - Not now, ask later\n" +
                "[Cancel] - DO NOT ask again at all\n\n" +
                "Your choice:");
            _translations.Add("ru_UpdateAvailableDialogNew",
                "доступна новая версия\n\n" +
                "[Да] - Открыть страницу обновления в браузере\n" +
                "[Нет] - Не сейчас, спросить потом\n" +
                "[Отмена] - Больше НЕ спрашивать вообще\n\n" +
                "Ваш выбор:");

            _translations.Add("en_UpdateDeferred", "[UPD]: Update notifications disabled. Delete update_deferral file to re-enable.");
            _translations.Add("ru_UpdateDeferred", "[UPD]: Уведомления об обновлениях отключены. Удалите файл update_deferral для включения.");

            _translations.Add("en_InfoUseIpsetFile", "[Info]: Use ipset file: {0}");
            _translations.Add("ru_InfoUseIpsetFile", "[Информация]: Используется ipset файл: {0}");

            _translations.Add("en_IpsetStatusEnabled", "Enabled");
            _translations.Add("ru_IpsetStatusEnabled", "Включено");

            _translations.Add("en_IpsetStatusStub", "Stub");
            _translations.Add("ru_IpsetStatusStub", "Заглушка");

            _translations.Add("en_InfoGoDisableIpset", "[Info]: Disabling ipset (use a stub)");
            _translations.Add("ru_InfoGoDisableIpset", "[Информация]: Выключаем ipset (используем заглушку)");

            _translations.Add("en_InfoGoUseIpset", "[Info]: Using the full ipset list file...");
            _translations.Add("ru_InfoGoUseIpset", "[Информация]: Используем полноценный файл листа ipset...");

            _translations.Add("en_InfoIpsetDownloading", "[Info]: Downloading ipset list of networks...");
            _translations.Add("ru_InfoIpsetDownloading", "[Информация]: Скачиваем ipset лист сетей...");

            _translations.Add("en_InfoIpsetUpdateOk", "[Info]: ipset list has been successfully updated!");
            _translations.Add("ru_InfoIpsetUpdateOk", "[Информация]: Список ipset успешно обновлен!");

            _translations.Add("en_ErrorIpsetUpdate", "[Error]: Error updating ipset list: {0}");
            _translations.Add("ru_ErrorIpsetUpdate", "[Ошибка]: Ошибка обновления ipset листа: {0}");

            _translations.Add("en_InfoExtendedPortsModeFilter", "[Info]: Extended ports mode (games mode): {0}");
            _translations.Add("ru_InfoExtendedPortsModeFilter", "[Информация]: Расширенный режим портов (games mode): {0}");

            _translations.Add("en_ExtendedPortsModeEnabled", "Enabled (ports 1024-65535)");
            _translations.Add("ru_ExtendedPortsModeEnabled", "Включен (порты 1024-65535)");

            _translations.Add("en_ExtendedPortsModeDisabled", "Disabled");
            _translations.Add("ru_ExtendedPortsModeDisabled", "Выключен");

            _translations.Add("en_InfoEnablingExtendedPortsMode", "[Info]: Enabling... (the extended_ports_mode file is created).");
            _translations.Add("ru_InfoEnablingExtendedPortsMode", "[Информация]: Включение... (создание файла extended_ports_mode).");

            _translations.Add("en_InfoDisablingExtendedPortsMode", "[Info]: Disabling... (deleting the extended_ports_mode file).");
            _translations.Add("ru_InfoDisablingExtendedPortsMode", "[Информация]: Выключние... (удаление файла extended_ports_mode).");


            _translations.Add("en_ErrorMissingCommand", "[Error]: Missing required command: {0}");
            _translations.Add("ru_ErrorMissingCommand", "[Ошибка]: Отсутствует необходимая команда: {0}");

            _translations.Add("en_InfoInstallUtilityHelp",
                "Please install: sudo apt install nftables procps (Debian / Ubuntu)\n" +
                "                sudo dnf install nftables procps-ng (Fedora / RHEL)\n" +
                "                sudo pacman -S nftables procps-ng (Arch / Manjaro)");
            _translations.Add("ru_InfoInstallUtilityHelp",
                "Пожалуйста, установите: sudo apt install nftables procps (Debian / Ubuntu)\n" +
                "                        sudo dnf install nftables procps-ng (Fedora / RHEL)\n" +
                "                        sudo pacman -S nftables procps-ng (Arch / Manjaro)");

            _translations.Add("en_ErrorCommandFailed", "[Warning]: Command failed: {0}\nError: {1}");
            _translations.Add("ru_ErrorCommandFailed", "[Предупреждение]: Команда не выполнена: {0}\nОшибка: {1}");

            _translations.Add("en_ErrorSudoCommandFailed", "[Error]: Failed to run sudo command: '{0}': {1}");
            _translations.Add("ru_ErrorSudoCommandFailed", "[Ошибка]: Не удалось запустить команду sudo: '{0}': {1}");

            _translations.Add("en_WarnPkillNfqws", "[Warning]: Failed to stop nfqws with pkill, trying sudo...");
            _translations.Add("ru_WarnPkillNfqws", "[Предупреждение]: Не удалось остановить nfqws с помощью pkill, пробуем через sudo...");

            _translations.Add("en_ErrorStopNfqws", "[Error]: Error stopping nfqws: {0}");
            _translations.Add("ru_ErrorStopNfqws", "[Ошибка]: Ошибка остановки nfqws: {0}");

            _translations.Add("en_InfoNftExtendedPortsEnabled", "[Info]: nftables - extended ports mode enabled (1024-65535)");
            _translations.Add("ru_InfoNftExtendedPortsEnabled", "[Информация]: nftables - включен режим расширенных портов (1024-65535)");

            _translations.Add("en_InfoNfqwsAlreadyRunning",
                "[Warning]: nfqws process is already running\n" +
                "[Info]: Stopping existing process and cleaning rules...");
            _translations.Add("ru_InfoNfqwsAlreadyRunning",
                "[Предупреждение]: Процесс nfqws уже запущен\n" +
                "[Информация]: Остановка существующих процессов и очистка правил...");

            _translations.Add("en_InfoVerifyNfqwsRun", "[Info]: nfqws is running.");
            _translations.Add("ru_InfoVerifyNfqwsRun", "[Информация]: nfqws запущен.");

            _translations.Add("en_ErrorVerifyNfqwsRun", "[Error]: (check) nfqws failed to start.");
            _translations.Add("ru_ErrorVerifyNfqwsRun", "[Ошибка]: (проверка) Не удалось запустить nfqws.");

            _translations.Add("en_ErrorFirstVerifyNfqwsRun", "[Error]: nfqws failed to start. Please check sudo permissions.");
            _translations.Add("ru_ErrorFirstVerifyNfqwsRun", "[Ошибка]: Не удалось запустить nfqws. Пожалуйста, проверьте права sudo.");

            _translations.Add("en_ErrorConfigureNft", "[Error]: Failed to configure nftables: {0}");
            _translations.Add("ru_ErrorConfigureNft", "[Ошибка]: Не удалось настроить nftables: {0}");

            _translations.Add("en_ErrorConfigureNftExit", "[Error]: Failed to configure nftables. Stopping...");
            _translations.Add("ru_ErrorConfigureNftExit", "[Ошибка]: Не удалось настроить nftables. Остановка...");

            _translations.Add("en_InfoCurrentNftCofig", "[Info]: Current nftables configuration:");
            _translations.Add("ru_InfoCurrentNftCofig", "[Информация]: Текущая конфигурация nftables:");

            _translations.Add("en_InfoAddedTcpPortRules", "[Info]: Added TCP rules for ports: {0}");
            _translations.Add("ru_InfoAddedTcpPortRules", "[Информация]: Добавлены TCP правила для портов: {0}");

            _translations.Add("en_InfoAddedUdpPortRules", "[Info]: Added UDP rules for ports: {0}");
            _translations.Add("ru_InfoAddedUdpPortRules", "[Информация]: Добавлены UDP правила для портов: {0}");

            _translations.Add("en_WarnAddTcpRulesFailed", "[Warning]: Failed to add TCP rules");
            _translations.Add("ru_WarnAddTcpRulesFailed", "[Предупреждение]: Не удалось добавить правила TCP.");

            _translations.Add("en_WarnAddUdpRulesFailed", "[Warning]: Failed to add UDP rules");
            _translations.Add("ru_WarnAddUdpRulesFailed", "[Предупреждение]: Не удалось добавить правила UDP.");

            _translations.Add("en_ErrorCreateNftChain", "[Error]: Failed to create nft chain.");
            _translations.Add("ru_ErrorCreateNftChain", "[Ошибка]: не удалось создать цепочку nft.");

            _translations.Add("en_ErrorCreateNftTable", "[Error]: Failed to create nft table.");
            _translations.Add("ru_ErrorCreateNftTable", "[Ошибка]: не удалось создать таблицу nft.");

            _translations.Add("en_InfoStoppingNfqws", "[Info]: Stopping nfqws process...");
            _translations.Add("ru_InfoStoppingNfqws", "[Информация]: Останавливаем процесс nfqws...");

            _translations.Add("en_SuccessNfqwsStopped", "[Success]: nfqws stopped.");
            _translations.Add("ru_SuccessNfqwsStopped", "[Успешно]: nfqws остановлен.");

            _translations.Add("en_InfoCleaningNftRules", "[Info]: Cleaning nftables rules...");
            _translations.Add("ru_InfoCleaningNftRules", "[Информация]: Очищаем правила nftables...");

            _translations.Add("en_SuccessNftRulesRemoved", "[Success]: nftables rules removed.");
            _translations.Add("ru_SuccessNftRulesRemoved", "[Успешно]: Правила nftables удалены.");

            _translations.Add("en_ErrorCleanNftRules", "[Error]: Error cleaning nftable rules: {0}");
            _translations.Add("ru_ErrorCleanNftRules", "[Ошибка]: Ошибка очистки nftable правил: {0}");

            _translations.Add("en_InfoSettingNftRules", "[Info]: Setting up nftables rules...");
            _translations.Add("ru_InfoSettingNftRules", "[Информация]: Настраиваем правила nftables...");

            _translations.Add("en_InfoAllStopAndCleanRules", "[Info]: All processes stopped and rules cleaned.");
            _translations.Add("ru_InfoAllStopAndCleanRules", "[Информация]: Все процессы остановлены и правила очищены.");

            _translations.Add("en_SuccessNftRulesConfigured", "[Success]: nftables rules configured.");
            _translations.Add("ru_SuccessNftRulesConfigured", "[Успешно]: Правила nftables настроены.");

            _translations.Add("en_InfoSuccessRunWithRules",
                "[Success]: liberty-pre is running with nftables rules\n" +
                "[Info]: Press Ctrl+C to stop (rules will be cleaned automatically).");
            _translations.Add("ru_InfoSuccessRunWithRules",
                "[Успешно]: liberty-pre запущен с nftables правилами\n" +
                "[Информация]: Нажмите Ctrl+C, чтобы остановить (правила будут очищены автоматически).");

            _translations.Add("en_InfoCleanupComletedExit", "[Info]: Cleanup completed. Exiting.");
            _translations.Add("ru_InfoCleanupComletedExit", "[Информация]: Очистка завершена. Выход.");
        }

        // Метод для вывода переведенного текста с форматированием
        // Method for outputting translated text with formattin
        public static void WriteTr(string textKey, params object[] args)
        {
            // Определение текущей культуры
            // Determine current culture
            var culture = CultureInfo.CurrentCulture.Name;

            string translationKey;
            // Выбор языка на основе культуры (ru или en)
            // Select language based on culture (ru or en)
            if (culture.StartsWith("ru"))
            {
                translationKey = $"ru_{textKey}";
            }
            else
            {
                translationKey = $"en_{textKey}";
            }

            // Поиск перевода в словаре
            // Search for translation in dictionary
            if (_translations.ContainsKey(translationKey))
            {
                string translatedText = _translations[translationKey];
                // Форматирование строки с аргументами
                // Format string with arguments
                Console.WriteLine(string.Format(translatedText, args));
            }
            else
            {
                // Fallback: вывод оригинального ключа / output of the original key
                Console.WriteLine(string.Format(textKey, args));
            }
        }

        // Метод для получения переведенной строки (без вывода в консоль)
        // Method for getting translated string (without console output)
        public static string GetStrTr(string textKey)
        {
            var culture = CultureInfo.CurrentCulture.Name;

            string translationKey;
            if (culture.StartsWith("ru"))
            {
                translationKey = $"ru_{textKey}";
            }
            else
            {
                translationKey = $"en_{textKey}";
            }

            // Если перевод не найден, возвращаем ключ
            // If translation not found, return the key
            if (!_translations.ContainsKey(translationKey))
            {
                return textKey;
            }
            string translated = _translations[translationKey];
            return translated;
        }
    }
}
