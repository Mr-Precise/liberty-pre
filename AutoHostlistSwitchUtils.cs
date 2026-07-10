using System;
using System.IO;

namespace libertypre
{
    public class AutoHostlistSwitchUtils
    {
        // Класс для переключения состояния hostlist-auto (автоматичекое добавление хостов если они заблокированы)
        // Class for toggling the state of hostlist-auto (automatic addition of hosts if they are blocked)

        private static string FlagFile = Path.Combine(MainClass.basePath, "hostlist_auto");
        private static string AutoHostlistFile = Path.Combine(MainClass.dataPath, "hostlist-auto.txt");

        // Получить состояние hostlist-auto
        // Get the status of hostlist-auto
        public static bool GetStatus()
        {
            return File.Exists(FlagFile);
        }

        // Переключатель состояния hostlist-auto
        // Toggle hostlist-auto state
        public static void Toggle()
        {
            if (GetStatus())
            {
                File.Delete(FlagFile);
                LocaleUtils.WriteTr("InfoDisablingHostlistAuto");
            }
            else
            {
                File.WriteAllText(FlagFile, "");
                LocaleUtils.WriteTr("InfoEnablingHostlistAuto");
                EnsureHostlistFileExists();
            }
        }

        // Создаем hostlist-auto.txt если его нет, чтобы избежать ошибок при запуске
        // Create hostlist-auto.txt if it doesn't exist to avoid errors on launch
        private static void EnsureHostlistFileExists()
        {
            if (!File.Exists(AutoHostlistFile))
            {
                try
                {
                    // Создаем файл с локализованным комментарием о том, что его можно чистить
                    // Create the file with a localized comment that it can be cleaned
                    File.WriteAllText(AutoHostlistFile, LocaleUtils.GetStrTr("HostlistAutoFileComment"));
                    LocaleUtils.WriteTr("InfoHostlistAutoFileCreated");
                }
                catch (Exception ex)
                {
                    LocaleUtils.WriteTr("ErrorCreatingHostlistAutoFile", ex.Message);
                }
            }
        }

        // Метод для получения строки аргумента hostlist-auto
        // Method to get the hostlist-auto argument string
        public static string GetArgumentString()
        {
            if (GetStatus())
            {
                EnsureHostlistFileExists();
                if (!File.Exists(AutoHostlistFile))
                {
                    // Если создать файл не удалось - не добавляем строку
                    // If creation failed - do not add the string
                    return "";
                }
                // Возвращаем строку с аргументом
                // Return the string with the argument
                return "--hostlist-auto=\"hostlist-auto.txt\"";
            }
            return "";
        }
    }
}
