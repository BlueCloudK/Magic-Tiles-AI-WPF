using BlueCloudK.WpfMusicTilesAI.Models;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Service for managing application settings
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Gets the current settings
        /// </summary>
        AppSettings Settings { get; }

        /// <summary>
        /// Loads settings from disk
        /// </summary>
        Task LoadSettingsAsync();

        /// <summary>
        /// Saves settings to disk
        /// </summary>
        Task SaveSettingsAsync();

        /// <summary>
        /// Resets settings to defaults
        /// </summary>
        Task ResetToDefaultsAsync();
    }
}
