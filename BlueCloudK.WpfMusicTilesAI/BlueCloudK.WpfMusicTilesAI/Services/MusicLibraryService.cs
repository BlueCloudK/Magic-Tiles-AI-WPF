using BlueCloudK.WpfMusicTilesAI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Service for managing the user's music library
    /// </summary>
    public class MusicLibraryService : IMusicLibraryService
    {
        private readonly string _libraryFilePath;
        private readonly IBeatMapCacheService _beatMapCache;
        private MusicLibrary _library;

        public MusicLibraryService(IBeatMapCacheService beatMapCache)
        {
            _beatMapCache = beatMapCache ?? throw new ArgumentNullException(nameof(beatMapCache));

            // Store library in AppData/Local/MagicTilesAI/library.json
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MagicTilesAI"
            );

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _libraryFilePath = Path.Combine(appDataPath, "library.json");
            _library = new MusicLibrary();
        }

        public List<LocalSong> GetAllSongs()
        {
            return _library.Songs;
        }

        public async Task<LocalSong> AddSongAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Audio file not found", filePath);

            // Check if song already exists
            var existingSong = _library.Songs.FirstOrDefault(s => s.FilePath == filePath);
            if (existingSong != null)
                return existingSong;

            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var songId = Guid.NewGuid().ToString();

            var song = new LocalSong
            {
                Id = songId,
                Title = fileName, // User can edit later
                Artist = "Unknown Artist",
                FilePath = filePath,
                FileName = fileName,
                DateAdded = DateTime.Now,
                Url = filePath,
                Description = $"A song titled {fileName}",
                HasBeatMap = _beatMapCache.HasBeatMap(songId),
                BeatMapPath = _beatMapCache.HasBeatMap(songId) ? _beatMapCache.GetBeatMapPath(songId) : null
            };

            _library.Songs.Add(song);
            await SaveLibraryAsync();

            return song;
        }

        public async Task RemoveSongAsync(string songId)
        {
            var song = _library.Songs.FirstOrDefault(s => s.Id == songId);
            if (song != null)
            {
                _library.Songs.Remove(song);

                // Optionally delete beat map
                if (song.HasBeatMap)
                {
                    await _beatMapCache.DeleteBeatMapAsync(songId);
                }

                await SaveLibraryAsync();
            }
        }

        public LocalSong? GetSong(string songId)
        {
            return _library.Songs.FirstOrDefault(s => s.Id == songId);
        }

        public async Task UpdateSongAsync(LocalSong song)
        {
            var index = _library.Songs.FindIndex(s => s.Id == song.Id);
            if (index >= 0)
            {
                _library.Songs[index] = song;
                await SaveLibraryAsync();
            }
        }

        public async Task SaveLibraryAsync()
        {
            _library.LastUpdated = DateTime.Now;
            var json = JsonConvert.SerializeObject(_library, Formatting.Indented);
            await File.WriteAllTextAsync(_libraryFilePath, json);
        }

        public async Task LoadLibraryAsync()
        {
            try
            {
                if (File.Exists(_libraryFilePath))
                {
                    var json = await File.ReadAllTextAsync(_libraryFilePath);
                    _library = JsonConvert.DeserializeObject<MusicLibrary>(json) ?? new MusicLibrary();

                    // Update HasBeatMap status for all songs
                    foreach (var song in _library.Songs)
                    {
                        song.HasBeatMap = _beatMapCache.HasBeatMap(song.Id);
                        if (song.HasBeatMap)
                        {
                            song.BeatMapPath = _beatMapCache.GetBeatMapPath(song.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load library: {ex.Message}");
                _library = new MusicLibrary();
            }
        }

        public async Task RecordPlayAsync(string songId)
        {
            var song = GetSong(songId);
            if (song != null)
            {
                song.PlayCount++;
                song.LastPlayed = DateTime.Now;
                await UpdateSongAsync(song);
            }
        }
    }
}
