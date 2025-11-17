using BlueCloudK.WpfMusicTilesAI.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Service for caching beat maps to disk
    /// </summary>
    public class BeatMapCacheService : IBeatMapCacheService
    {
        private readonly string _cacheDirectory;

        public BeatMapCacheService()
        {
            // Store beat maps in AppData/Local/MagicTilesAI/BeatMaps
            _cacheDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MagicTilesAI",
                "BeatMaps"
            );

            // Create directory if it doesn't exist
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }
        }

        public async Task<string> SaveBeatMapAsync(string songId, BeatMap beatMap)
        {
            var filePath = GetBeatMapPath(songId);
            var json = JsonConvert.SerializeObject(beatMap, Formatting.Indented);

            await File.WriteAllTextAsync(filePath, json);

            return filePath;
        }

        public async Task<BeatMap?> LoadBeatMapAsync(string beatMapPath)
        {
            try
            {
                if (!File.Exists(beatMapPath))
                    return null;

                var json = await File.ReadAllTextAsync(beatMapPath);
                return JsonConvert.DeserializeObject<BeatMap>(json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load beat map: {ex.Message}");
                return null;
            }
        }

        public bool HasBeatMap(string songId)
        {
            var filePath = GetBeatMapPath(songId);
            return File.Exists(filePath);
        }

        public string GetBeatMapPath(string songId)
        {
            // Sanitize song ID for file name
            var safeFileName = string.Join("_", songId.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(_cacheDirectory, $"{safeFileName}.json");
        }

        public async Task DeleteBeatMapAsync(string songId)
        {
            var filePath = GetBeatMapPath(songId);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }
    }
}
