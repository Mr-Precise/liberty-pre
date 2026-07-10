using System;
using System.Net.Http;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Net;

namespace libertypre
{
    public class UpdCheck
    {
        // Событие для синхронизации завершения проверки
        // Event for synchronization of check completion
        public static ManualResetEvent DoneEvent = new ManualResetEvent(false);

        private const string RepoOwner = "Mr-Precise";
        private const string RepoName = "liberty-pre";
        private static string UpdateFlagFile = Path.Combine(MainClass.basePath, "update_deferral");

        // Основной и резервный URL для получения информации о последнем релизе
        // Primary and fallback URLs for getting latest release information
        private const string PrimaryApiUrl = "https://api.github.com/repos/{0}/{1}/releases/latest";
        private const string FallbackApiUrl = "https://liberty-pre.rapid-waterfall-3845.workers.dev/repos/{0}/{1}/releases/latest";

        // Метод проверки обновлений
        // Update check method
        public static async Task CheckForUpdAsync()
        {
            try
            {
                // Устанавливаем TLS 1.2 для безопасного соединения
                // Требуется этоn фикс для правильной работы на .net Framework 4.5
                // Set TLS 1.2 for secure connection
                // This is a necessary fix for proper operation on .NET Framework 4.5
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                // Получаем текущую версию приложения
                // Get current application version
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

                // Пытаемся получить данные сначала с основного URL, потом с резервного
                // Try to get data from primary URL, then from fallback
                JObject release = FetchReleaseDataWithFallback();
                if (release == null)
                {
                    // Если оба источника не сработали, ошибка уже выведена внутри
                    // If both sources failed, the error has already been logged inside
                    return;
                }

                // Извлекаем тег версии
                // Extract version tag
                var latestTag = release["tag_name"]?.ToString();
                var latestVersion = latestTag?.TrimStart('v');
                var htmlUrl = release["html_url"]?.ToString() ?? $"https://github.com/{RepoOwner}/{RepoName}/releases";

                // Пишем версии используя локализацию и цвета
                // Write versions using localization and colors
                Console.Write("[UPD]: ");
                Console.Write(LocaleUtils.GetStrTr("UpdCheckLocal"));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(currentVersion);
                Console.ResetColor();
                Console.Write(LocaleUtils.GetStrTr("UpdCheckRemote"));
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(latestVersion ?? "unknown");
                Console.ResetColor();

                // Сравниваем версии
                // Compare versions
                if (IsNewerVersion(latestVersion, currentVersion))
                {
                    ShowUpdDialog(latestVersion, htmlUrl);
                }
                else
                {
                    // Актуально
                    // Latest
                    LocaleUtils.WriteTr("UpdCheckLatest");
                }
            }
            catch (Exception ex)
            {
                LocaleUtils.WriteTr("UpdCheckFailed", ex.Message);
            }
            finally
            {
                // Сигнализируем о завершении события
                // Signal completion of the event
                DoneEvent.Set();
            }
        }

        // Получение данных с основного источника, при ошибке - с резервного
        // Get data from primary source, on error - from fallback
        private static JObject FetchReleaseDataWithFallback()
        {
            string primaryUrl = string.Format(PrimaryApiUrl, RepoOwner, RepoName);
            string fallbackUrl = string.Format(FallbackApiUrl, RepoOwner, RepoName);

            try
            {
                var result = FetchReleaseData(primaryUrl);
                if (result != null)
                {
                    return result;
                }
                // Если primaryResult == null (например, пре-релиз) - просто возвращаем null, не пробуем fallback
                // If primaryResult == null (e.g., pre-release) - just return null, do not try fallback
                return null;
            }
            catch (Exception ex)
            {
                // Основной источник недоступен
                // Primary source unavailable
                LocaleUtils.WriteTr("UpdCheckPrimaryFailed");
                LocaleUtils.WriteTr("UpdCheckPrimaryError", ex.Message);

                try
                {
                    var fallbackResult = FetchReleaseData(fallbackUrl);
                    if (fallbackResult != null)
                    {
                        LocaleUtils.WriteTr("UpdCheckFallbackUsed");
                        return fallbackResult;
                    }
                    // если fallbackResult == null (пре-релиз) - тоже возвращаем null
                    // if fallbackResult == null (pre-release) - also return null
                    return null;
                }
                catch (Exception fallbackEx)
                {
                    // Оба источника не работают - возвращаем null
                    // Both sources failed - return null
                    LocaleUtils.WriteTr("UpdCheckBothFailed");
                    LocaleUtils.WriteTr("UpdCheckFallbackError", fallbackEx.Message);
                    return null;
                }
            }
        }

        // Получение данных с указанного URL.
        // Get data from specified URL.
        private static JObject FetchReleaseData(string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd($"{RepoName}-upd-checker");
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                var json = response.Content.ReadAsStringAsync().Result;
                var release = JObject.Parse(json);

                return release;
            }
        }

        // Сравнение версий (семантическое версионирование)
        // Version comparison (semantic versioning)
        private static bool IsNewerVersion(string? latest, string? current)
        {
            if (string.IsNullOrEmpty(latest) || string.IsNullOrEmpty(current))
                return false;

            try
            {
                // Парсим и сравниваем Version объекты
                // Parse and compare Version objects
                var latestVersion = Version.Parse(latest);
                var currentVersion = Version.Parse(current);
                return latestVersion > currentVersion;
            }
            catch
            {
                return false;
            }
        }

        // Показ диалога обновления с обработкой отсрочек
        // Show update dialog with deferral handling
        private static void ShowUpdDialog(string version, string updUrl)
        {
            // Проверяем наличие файла для пропуска уведомлений
            // Check for file to skip notifications
            if (File.Exists(UpdateFlagFile))
            {
                LocaleUtils.WriteTr("UpdateSkip");
                return;
            }

            // На Linux если без GUI или по SSH - показываем только в консоли
            // On Linux if no GUI or via SSH - show only in console
            if (GeneralUtils.IsLinux() && !GeneralUtils.HasDisplayServer())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                LocaleUtils.WriteTr("UpdCheckNoDisplay", version, updUrl);
                Console.ResetColor();
                return;
            }

            ShowMessageBoxDialog(version, updUrl);
        }

        private static void ShowMessageBoxDialog(string version, string updUrl)
        {
            // Показываем диалоговое окошко обновления
            // Show the update dialog
            var result = MessageBox.Show(
                version + " - " +
                LocaleUtils.GetStrTr("UpdateAvailableDialogNew"),
                LocaleUtils.GetStrTr("UpdateAvailableDialog"),
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                // Открываем страницу обновления
                // Open update page
                Process.Start(new ProcessStartInfo
                {
                    FileName = updUrl,
                    UseShellExecute = true
                });
            }
            else if (result == DialogResult.Cancel)
            {
                // Создаем пустой файл-флаг для отключения уведомлений
                // Create empty flag file to disable notifications
                try
                {
                    File.WriteAllText(UpdateFlagFile, "");
                    LocaleUtils.WriteTr("UpdateDeferred");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
