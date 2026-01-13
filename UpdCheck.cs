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

        // Метод проверки обновлений
        // Update check method
        public static async Task CheckForUpdAsync()
        {
            try
            {
                // Получаем текущую версию приложения
                // Get current application version
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

                var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd($"{RepoName}-upd-checker");

                // Получаем информацию о последнем релизе
                // Get latest release information
                var response = await client.GetAsync($"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest");
                response.EnsureSuccessStatusCode();

                // Парсим JSON ответ
                // Parse JSON response
                var json = await response.Content.ReadAsStringAsync();
                var release = JObject.Parse(json);

                // Пропускаем пре-релизы и черновики
                // Skip pre-releases and drafts
                if (release["prerelease"]?.Value<bool>() == true || release["draft"]?.Value<bool>() == true)
                    return;

                // Извлекаем тег версии
                // Extract version tag
                var latestTag = release["tag_name"]?.ToString();
                var latestVersion = latestTag?.TrimStart('v');

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
                    var url = release["html_url"]?.ToString() ?? $"https://github.com/{RepoOwner}/{RepoName}/releases";
                    ShowUpdDialog(latestVersion, url!);
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
