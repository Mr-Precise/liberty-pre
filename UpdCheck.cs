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
        private static string DeferralConfig = Path.Combine(MainClass.basePath, "update_deferral.json");

        // Период отсрочки
        // Deferral period
        private static readonly TimeSpan DeferralDuration = TimeSpan.FromDays(2);

        // Метод проверки обновлений
        // Update check method
        public static async Task CheckForUpdAsync()
        {
            try
            {
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

                LocaleUtils.WriteTr("UpdCheckRemoteVer", latestVersion);

                // Получаем текущую версию приложения
                // Get current application version
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

                // Сравниваем версии
                // Compare versions
                if (IsNewerVersion(latestVersion, currentVersion))
                {
                    var url = release["html_url"]?.ToString() ?? $"https://github.com/{RepoOwner}/{RepoName}/releases";
                    ShowUpdDialog(latestVersion, url!);
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
            if (File.Exists(DeferralConfig))
            {
                try
                {
                    string deferralText = File.ReadAllText(DeferralConfig);
                    if (string.IsNullOrWhiteSpace(deferralText))
                    {
                        File.Delete(DeferralConfig);
                    }
                    else
                    {
                        var deferralJson = JObject.Parse(deferralText);
                        var nextTime = deferralJson["nextReminder"]?.ToObject<DateTime>() ?? DateTime.MinValue;

                        // Пропускаем показ, если период отсрочки не истек
                        // Skip if deferral period hasn't expired
                        if (DateTime.Now < nextTime)
                        {
                            LocaleUtils.WriteTr("UpdateSkip");
                            return;
                        }
                        else
                            File.Delete(DeferralConfig);
                    }
                }
                catch
                {
                    // Удаляем битый конфиг
                    // Remove corrupted config
                    File.Delete(DeferralConfig);
                }
            }

            // Показываем диалог с тремя вариантами
            // Show dialog with three options
            var result = MessageBox.Show(
                LocaleUtils.GetStrTr("UpdateAvailableNewVer") + version,
                LocaleUtils.GetStrTr("UpdateAvailableDialog"),
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                // Открываем страницу релиза
                // Open release page
                Process.Start(new ProcessStartInfo
                {
                    FileName = updUrl,
                    UseShellExecute = true
                });
            }
            else if (result == DialogResult.Cancel)
            {
                // Создаем / обновляем конфиг отсрочки
                // Create / update deferral config
                var newDeferral = new JObject
                {
                    ["nextReminder"] = DateTime.Now.Add(DeferralDuration)
                };
                File.WriteAllText(DeferralConfig, newDeferral.ToString());
            }
        }
    }
}
