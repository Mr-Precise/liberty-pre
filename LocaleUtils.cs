using System;
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

            _translations.Add("en_WarningLinux", "[Warning]: For Linux/macOS use original zapret https://github.com/bol-van/zapret");
            _translations.Add("ru_WarningLinux", "[Предупреждение]: Для Linux/macOS используйте оригинальный zapret https://github.com/bol-van/zapret");

            _translations.Add("en_ErrorBinNotFound", "[Error]: The bin directory not found.");
            _translations.Add("ru_ErrorBinNotFound", "[Ошибка]: Каталог bin не найден.");

            _translations.Add("en_ErrorWinwsNotFound", "[Error]: File winws.exe not found in bin.");
            _translations.Add("ru_ErrorWinwsNotFound", "[Ошибка]: Файл winws.exe не найден в bin.");

            _translations.Add("en_WarningConfigFileNotSpecified", "[Warning]: Configuration file not specified, loading default.cfg");
            _translations.Add("ru_WarningConfigFileNotSpecified", "[Предупреждение]: Конфигурационный файл не указан, загружаем default.cfg");

            _translations.Add("en_ErrorWinwsAldeadyRunning", "[Error]: The process winws.exe is already running, please close the existing one!");
            _translations.Add("ru_ErrorWinwsAldeadyRunning", "[Ошибка]: Процесс winws.exe уже запущен, закройте существующий!");

            _translations.Add("en_DoneWinwsStarted", "[Done]: winws.exe started with configuration: {0}");
            _translations.Add("ru_DoneWinwsStarted", "[Готово]: winws.exe запущен с конфигурацией: {0}");

            _translations.Add("en_InfoWinwsMinimized", "[Info]: winws is minimized, this window will close itself in 3 seconds...");
            _translations.Add("ru_InfoWinwsMinimized", "[Информация]: winws теперь свёрнут, это окно само закроется через 3 секунды...");

            _translations.Add("en_ErrorProblemStartWinws", "[Error]: Problems starting winws.exe: {0}");
            _translations.Add("ru_ErrorProblemStartWinws", "[Ошибка]: Проблемы при запуске winws.exe: {0}");

            _translations.Add("en_ShowVersion", "liberty-pre zapret launcher {0}\nAuthor: {1}\nProject: {2}");
            _translations.Add("ru_ShowVersion", "liberty-pre zapret лаунчер {0}\nАвтор: {1}\nПроект: {2}");

            _translations.Add("en_ShowHelp", "liberty-pre.exe -c <config>.cfg\nLaunch examples:\n  liberty-pre.exe -c discord.cfg\n  liberty-pre.exe -c general.cfg\nNote: by default file used is default.cfg\nArguments:\n  -h, --help    Show help\n  -v            Show version");
            _translations.Add("ru_ShowHelp", "liberty-pre.exe -c <конфиг>.cfg\nПримеры запуска:\n  liberty-pre.exe -c discord.cfg\n  liberty-pre.exe -c general.cfg\nПримечание: по умолчанию читается файл default.cfg\nАргументы:\n  -h, --help    Показать справку\n  -v            Показать версию");
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
    }
}
