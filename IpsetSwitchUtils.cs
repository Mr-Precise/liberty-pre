using System;
using System.IO;
using System.Linq;
using System.Net;

namespace libertypre
{
    public class IpsetSwitchUtils
    {
        private static string dataPath = MainClass.dataPath;
        private static string currentFile = Path.Combine(dataPath, "ipset.txt");
        private static string stubFile = Path.Combine(dataPath, "ipset-stub.txt");
        private static string fullFile = Path.Combine(dataPath, "ipset-orig.txt");
        private static string backupFile = Path.Combine(dataPath, "ipset.txt.backup");

        // TEST-NET-3
        private const string stubIp = "203.0.113.113/32";

        public static void Initialize()
        {
            // Создаем заглушку если её нет
            // Create a stub if it doesn't exist
            if (!File.Exists(stubFile))
            {
                File.WriteAllText(stubFile, stubIp);
            }

            // Скачиваем полный файл если его нет
            // Download a full file if it does not exist
            if (!File.Exists(fullFile))
            {
                UpdateFullFile();
            }

            // Если текущего файла нет - копируем из полного
            // If there is no current file, copy from the full
            if (!File.Exists(currentFile))
            {
                File.Copy(fullFile, currentFile, true);
            }
        }

        // Переключает между полным листом и заглушкой
        // Возвращает true если после переключения полный активен
        // Switches between the full list and the stub
        // Returns true if the full list is active after switch
        public static bool IpsetSwitchNext()
        {
            Initialize();

            bool currentStatus = IpsetSwitchStatus();

            if (currentStatus)
            {
                // Если включено --> выключаем
                // If enabled --> disable 
                LocaleUtils.WriteTr("InfoGoDisableIpset");

                // Сохраняем текущий файл в backup на всякий случай
                // Save the current file to backup just in case
                File.Copy(currentFile, backupFile, true);

                // Копируем заглушку в текущий файл
                // Copy the stub to the current file
                File.Copy(stubFile, currentFile, true);

                return false;
            }
            else
            {
                // Если выключено --> включаем
                // If disabled --> enable
                LocaleUtils.WriteTr("InfoGoUseIpset");

                // Копируем полный файл в текущий
                // Copy the full file to the current
                File.Copy(fullFile, currentFile, true);

                return true;
            }
        }

        // Получаем состояние переключателя ipset
        // Get the status of the ipset switch
        public static bool IpsetSwitchStatus()
        {
            Initialize();

            if (!File.Exists(currentFile))
            {
                return false;
            }

            try
            {
                string content = File.ReadAllText(currentFile).Trim();

                // Сравниваем с заглушкой
                // Compare with a stub
                if (content == stubIp)
                {
                    return false;
                }

                // Если файл пустой - тоже выключено
                // If the file is empty - also disabled
                if (string.IsNullOrWhiteSpace(content))
                {
                    return false;
                }

                // Сравниваем с файлом-заглушкой
                // Compare with the stub file
                if (File.Exists(stubFile))
                {
                    string stubContent = File.ReadAllText(stubFile).Trim();
                    if (content == stubContent)
                    {
                        return false;
                    }
                }

                // Во всех остальных случаях считаем включенным
                // In all other cases we consider it enabled
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void UpdateFullFile(string url = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                // https://github.com/Mr-Precise/liberty-pre-configs/
                url = "https://raw.githubusercontent.com/Mr-Precise/liberty-pre-configs/main/ipset-orig.txt";
            }

            try
            {
                LocaleUtils.WriteTr("InfoIpsetDownloading");

                var client = new WebClient();
                string tempFile = Path.GetTempFileName();
                client.DownloadFile(url, tempFile);

                // Проверяем что файл не пустой
                // Check that the file is not empty
                string content = File.ReadAllText(tempFile);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    File.Copy(tempFile, fullFile, true);
                    LocaleUtils.WriteTr("InfoIpsetUpdateOk");

                    // Если полный ipset активен, сразу применяем обновленный лист
                    // If full ipset enabled, immediately apply the updated list
                    if (IpsetSwitchStatus() && !File.Exists(currentFile))
                    {
                        File.Copy(fullFile, currentFile, true);
                    }
                }

                File.Delete(tempFile);

            }
            catch (Exception ex)
            {
                // Если не удалось обновить полный и нет текущего, пробуем восстановить из backup
                // If it was not possible to update the full one and there is no current one, try restoring from backup
                if (!File.Exists(fullFile) && !File.Exists(currentFile) && File.Exists(backupFile))
                {
                    File.Copy(backupFile, currentFile, true);
                }
                LocaleUtils.WriteTr("ErrorIpsetUpdate", ex.Message);
            }
        }
    }
}
