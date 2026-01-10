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

            _translations.Add("en_ErrorConfigNotFound", "[Error]: Configuration file {0} not found.");
            _translations.Add("ru_ErrorConfigNotFound", "[Ошибка]: Конфигурационный файл {0} не найден.");

            _translations.Add("en_ErrorConfigEmpty", "[Error]: Configuration file is empty or contains only comments.");
            _translations.Add("ru_ErrorConfigEmpty", "[Ошибка]: Конфигурационный файл пуст или содержит только комментарии.");

            _translations.Add("en_WarningLinux", "[Warning]: For Linux/macOS recommended to use original zapret https://github.com/bol-van/zapret");
            _translations.Add("ru_WarningLinux", "[Предупреждение]: Для Linux/macOS рекомендуется использовать оригинальный zapret https://github.com/bol-van/zapret");

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

            _translations.Add("en_WarningConfigFileNotSpecified", "[Warning]: Configuration file not specified, loading default.cfg");
            _translations.Add("ru_WarningConfigFileNotSpecified", "[Предупреждение]: Конфигурационный файл не указан, загружаем default.cfg");

            _translations.Add("en_ErrorConfigFileNotFound", "[Error]: Configuration file {0} not found!");
            _translations.Add("ru_ErrorConfigFileNotFound", "[Ошибка]: Файл конфигурации {0} не найден!");

            _translations.Add("en_InfoUsedNfqws", "[Info]: Linux / Used nfqws (nftables)");
            _translations.Add("ru_InfoUsedNfqws", "[Информация]: Linux / Используется nfqws (nftables)");

            _translations.Add("en_InfoHiddenMode", "[Info]: Hidden mode (daemon) enabled. Run STOP shortcut for stop.");
            _translations.Add("ru_InfoHiddenMode", "[Информация]: Включен скрытый режим работы (daemon). Запустите ярлык STOP чтобы остановить.");

            _translations.Add("en_InfoUsedIptables", "[Info]: Linux / Used tpws (iptables), limited in functionality");
            _translations.Add("ru_InfoUsedIptables", "[Информация]: Linux / Используется tpws (iptables), ограниченная по функционалу");

            _translations.Add("en_WarningAldeadyRunning", "[Warning]: The process has already been running, restarting! To full stop, run the shortcut liberty-pre STOP");
            _translations.Add("ru_WarningAldeadyRunning", "[Предупреждение]: Процесс уже был запущен, выполняется перезапуск! Чтобы полностью остановить запустите ярлык liberty-pre STOP");

            _translations.Add("en_DoneWinwsStarted", "[Done]: Started with configuration: {0}");
            _translations.Add("ru_DoneWinwsStarted", "[Готово]: Запущено с конфигурацией: {0}");

            _translations.Add("en_InfoWinwsMinimized", "[Info]: winws is minimized, this window will close itself in 3 seconds...");
            _translations.Add("ru_InfoWinwsMinimized", "[Информация]: winws теперь свёрнут, это окно само закроется через 3 секунды...");

            _translations.Add("en_ErrorProblemStartWinws", "[Error]: Problems starting winws.exe: {0}");
            _translations.Add("ru_ErrorProblemStartWinws", "[Ошибка]: Проблемы при запуске winws.exe: {0}");

            _translations.Add("en_ShowVersion", "liberty-pre zapret launcher {0}\nAuthor: {1}\nProject: {2}");
            _translations.Add("ru_ShowVersion", "liberty-pre zapret лаунчер {0}\nАвтор: {1}\nПроект: {2}");

            _translations.Add("en_ShowHelp", "liberty-pre.exe -c <config>.cfg\nLaunch examples:\n  liberty-pre.exe -c discord.cfg\n  liberty-pre.exe -c general.cfg\nNote: by default file used is default.cfg\nArguments:\n  -h, --help       Show help\n  -c               Path to config file\n  -i               Use iptables instead of nftables\n  -s, --stop       Stop winws and delete WinDivert Service\n  --ipset          Switch ipset file mode (full or stub)\n  --extended-ports Switch extended ports mode (port filtering 1024-65535)\n  -v               Show version");
            _translations.Add("ru_ShowHelp", "liberty-pre.exe -c <конфиг>.cfg\nПримеры запуска:\n  liberty-pre.exe -c discord.cfg\n  liberty-pre.exe -c general.cfg\nПримечание: по умолчанию читается файл default.cfg\nАргументы:\n  -h, --help       Показать справку\n  -c               Путь к файлу конфигурации\n  -i               Использовать iptables вместо nftables\n  -s, --stop       Остановить winws и удалить WinDivert сервис\n  --ipset          Переключатель режима работы ipset (полный или заглушка)\n  --extended-ports Переключатель расширенного режима портов (фильтрация портов 1024-65535)\n  -v               Показать версию");

            _translations.Add("en_UpdCheckFailed", "[Error]: Error when checking updates: {0}");
            _translations.Add("ru_UpdCheckFailed", "[Ошибка]: Ошибка при проверке обновлений: {0}");

            _translations.Add("en_CreateShortcutsFailed", "[Error]: Error while creating shortcuts (.lnk): {0}");
            _translations.Add("ru_CreateShortcutsFailed", "[Ошибка]: Ошибка при создании ярлыков (.lnk): {0}");

            _translations.Add("en_UpdCheckRemoteVer", "\n[UPD]: Checking the version on GitHub... current: {0}");
            _translations.Add("ru_UpdCheckRemoteVer", "\n[UPD]: Проверяем версию на GitHub... актуальная: {0}");

            _translations.Add("en_StopRemoveDrv", "[Done]: winws is stopped + WinDivert unload");
            _translations.Add("ru_StopRemoveDrv", "[Готово]: winws завершен + выгружен WinDivert");

            _translations.Add("en_UpdateSkip", "[UPD]: Update skip");
            _translations.Add("ru_UpdateSkip", "[UPD]: Обновление пропущено");

            _translations.Add("en_UpdateAvailableDialog", "Update Available");
            _translations.Add("ru_UpdateAvailableDialog", "Доступно обновление");

            _translations.Add("en_UpdateAvailableNewVer", "New version available.\n[Cancel] - postponing for 2 days.\n[Yes] - Open release page in browser?\n");
            _translations.Add("ru_UpdateAvailableNewVer", "Доступна новая версия.\n[Отмена] - отложить на 2 дня.\n[Да] - Открыть страницу релиза в браузере?\n");

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

            _translations.Add("en_InfoEnablingExtendedPortsMode", "[Info]: Enabling... (the extended_ports_mode file is created)");
            _translations.Add("ru_InfoEnablingExtendedPortsMode", "[Информация]: Включение... (создание файла extended_ports_mode)");

            _translations.Add("en_InfoDisablingExtendedPortsMode", "[Info]: Disabling... (deleting the extended_ports_mode file)");
            _translations.Add("ru_InfoDisablingExtendedPortsMode", "[Информация]: Выключние... (удаление файла extended_ports_mode)");
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
