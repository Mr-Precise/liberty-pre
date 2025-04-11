using System;
using System.Net.Http;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace libertypre
{
    public class UpdCheck
    {
        private const string RepoOwner = "Mr-Precise";
        private const string RepoName = "liberty-pre";
        private static string DeferralConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update_deferral.json");
        private static readonly TimeSpan DeferralDuration = TimeSpan.FromDays(2);

        public static async Task CheckForUpdAsync()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd($"{RepoName}-upd-checker");

                var response = await client.GetAsync($"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var release = JObject.Parse(json);

                if (release["prerelease"]?.Value<bool>() == true || release["draft"]?.Value<bool>() == true)
                    return;

                var latestTag = release["tag_name"]?.ToString();
                var latestVersion = latestTag?.TrimStart('v');

                LocaleUtils.WriteTr("UpdCheckRemoteVer", latestVersion);
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
                
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
        }

        private static bool IsNewerVersion(string? latest, string? current)
        {
            if (string.IsNullOrEmpty(latest) || string.IsNullOrEmpty(current))
                return false;

            try
            {
                var latestVersion = Version.Parse(latest);
                var currentVersion = Version.Parse(current);
                return latestVersion > currentVersion;
            }
            catch
            {
                return false;
            }
        }

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
                    File.Delete(DeferralConfig);
                }
            }

            var result = MessageBox.Show(
                LocaleUtils.GetStrTr("UpdateAvailableNewVer") + version,
                LocaleUtils.GetStrTr("UpdateAvailableDialog"),
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = updUrl,
                    UseShellExecute = true
                });
            }
            else if (result == DialogResult.Cancel)
            {
                var newDeferral = new JObject
                {
                    ["nextReminder"] = DateTime.Now.Add(DeferralDuration)
                };
                File.WriteAllText(DeferralConfig, newDeferral.ToString());
            }
        }
    }
}
