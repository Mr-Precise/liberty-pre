﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace libertypre
{
    public class LocaleUtils
    {
        private static Dictionary<string, string> _translations;

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

            _translations.Add("en_InfoUsedIptables", "[Info]: Linux / Used tpws (iptables), limited in functionality");
            _translations.Add("ru_InfoUsedIptables", "[Информация]: Linux / Используется tpws (iptables), ограниченная по функционалу");

            _translations.Add("en_ErrorWinwsAldeadyRunning", "[Error]: The process winws.exe is already running!, close the existing one or run the shortcut liberty-pre stop");
            _translations.Add("ru_ErrorWinwsAldeadyRunning", "[Ошибка]: Процесс winws.exe уже запущен!, закройте существующий или запустите ярлык liberty-pre stop");

            _translations.Add("en_DoneWinwsStarted", "[Done]: Started with configuration: {0}");
            _translations.Add("ru_DoneWinwsStarted", "[Готово]: Запущено с конфигурацией: {0}");

            _translations.Add("en_InfoWinwsMinimized", "[Info]: winws is minimized, this window will close itself in 3 seconds...");
            _translations.Add("ru_InfoWinwsMinimized", "[Информация]: winws теперь свёрнут, это окно само закроется через 3 секунды...");

            _translations.Add("en_ErrorProblemStartWinws", "[Error]: Problems starting winws.exe: {0}");
            _translations.Add("ru_ErrorProblemStartWinws", "[Ошибка]: Проблемы при запуске winws.exe: {0}");

            _translations.Add("en_ShowVersion", "liberty-pre zapret launcher {0}\nAuthor: {1}\nProject: {2}");
            _translations.Add("ru_ShowVersion", "liberty-pre zapret лаунчер {0}\nАвтор: {1}\nПроект: {2}");

            _translations.Add("en_ShowHelp", "liberty-pre.exe -c <config>.cfg\nLaunch examples:\n  liberty-pre.exe -c discord.cfg\n  liberty-pre.exe -c general.cfg\nNote: by default file used is default.cfg\nArguments:\n  -h, --help    Show help\n  -c            Path to config file\n  -i            Use iptables instead of nftables\n  -s, --stop    Stop winws and delete WinDivert Service\n  -v            Show version");
            _translations.Add("ru_ShowHelp", "liberty-pre.exe -c <конфиг>.cfg\nПримеры запуска:\n  liberty-pre.exe -c discord.cfg\n  liberty-pre.exe -c general.cfg\nПримечание: по умолчанию читается файл default.cfg\nАргументы:\n  -h, --help    Показать справку\n  -c            Путь к файлу конфирурации\n  -i            Использовать iptables вместо nftables\n  -s, --stop    Остановить winws и удалить WinDivert сервис\n  -v            Показать версию");

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

            // update dialog
            _translations.Add("en_UpdateAvailableDialog", "Update Available");
            _translations.Add("ru_UpdateAvailableDialog", "Доступно обновление");

            _translations.Add("en_UpdateAvailableNewVer", "New version available.\n[Cancel] - postponing for 2 days.\n[Yes] - Open release page in browser?\n");
            _translations.Add("ru_UpdateAvailableNewVer", "Доступна новая версия.\n[Отмена] - отложить на 2 дня.\n[Да] - Открыть страницу релиза в браузере?\n");
        }

        public static void WriteTr(string textKey, params object[] args)
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

            if (_translations.ContainsKey(translationKey))
            {
                string translatedText = _translations[translationKey];
                Console.WriteLine(string.Format(translatedText, args));
            }
            else
            {
                Console.WriteLine(string.Format(textKey, args));
            }
        }

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

            if (!_translations.ContainsKey(translationKey))
            {
                return textKey;
            }
            string translated = _translations[translationKey];
            return translated;
        }
    }
}
