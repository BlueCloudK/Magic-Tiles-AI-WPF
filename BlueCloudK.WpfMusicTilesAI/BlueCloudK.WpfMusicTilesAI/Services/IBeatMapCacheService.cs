using BlueCloudK.WpfMusicTilesAI.Models;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Service for caching beat maps to disk
    /// </summary>
    public interface IBeatMapCacheService
    {
        /// <summary>
        /// Saves a beat map to disk
        /// </summary>
        /// <param name="songId">Unique song identifier</param>
        /// <param name="beatMap">Beat map to save</param>
        /// <returns>Path to the saved beat map file</returns>
        Task<string> SaveBeatMapAsync(string songId, BeatMap beatMap);

        /// <summary>
        /// Loads a beat map from disk
        /// </summary>
        /// <param name="beatMapPath">Path to the beat map JSON file</param>
        /// <returns>The loaded beat map, or null if not found</returns>
        Task<BeatMap?> LoadBeatMapAsync(string beatMapPath);

        /// <summary>
        /// Checks if a beat map exists for a song
        /// </summary>
        /// <param name="songId">Unique song identifier</param>
        /// <returns>True if beat map exists</returns>
        bool HasBeatMap(string songId);

        /// <summary>
        /// Gets the path where a beat map would be saved for a song
        /// </summary>
        /// <param name="songId">Unique song identifier</param>
        /// <returns>Path to the beat map file</returns>
        string GetBeatMapPath(string songId);

        /// <summary>
        /// Deletes a cached beat map
        /// </summary>
        /// <param name="songId">Unique song identifier</param>
        Task DeleteBeatMapAsync(string songId);
    }
}
