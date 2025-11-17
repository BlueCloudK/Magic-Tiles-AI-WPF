using BlueCloudK.WpfMusicTilesAI.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Service for managing application settings
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsPath;
        private AppSettings _settings;

        public AppSettings Settings => _settings;

        public SettingsService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MagicTilesAI");

            Directory.CreateDirectory(appDataPath);
            _settingsPath = Path.Combine(appDataPath, "settings.json");

            _settings = new AppSettings();
        }

        /// <summary>
        /// Loads settings from disk
        /// </summary>
        public async Task LoadSettingsAsync()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = await File.ReadAllTextAsync(_settingsPath);
                    var loadedSettings = JsonConvert.DeserializeObject<AppSettings>(json);

                    if (loadedSettings != null)
                    {
                        _settings = loadedSettings;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
                // If loading fails, keep default settings
            }
        }

        /// <summary>
        /// Saves settings to disk
        /// </summary>
        public async Task SaveSettingsAsync()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
                await File.WriteAllTextAsync(_settingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Resets settings to defaults
        /// </summary>
        public async Task ResetToDefaultsAsync()
        {
            _settings = new AppSettings();
            await SaveSettingsAsync();
        }
    }
}
