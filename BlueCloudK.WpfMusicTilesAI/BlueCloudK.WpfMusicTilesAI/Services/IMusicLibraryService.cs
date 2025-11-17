using BlueCloudK.WpfMusicTilesAI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Service for managing the user's music library
    /// </summary>
    public interface IMusicLibraryService
    {
        /// <summary>
        /// Gets all songs in the library
        /// </summary>
        List<LocalSong> GetAllSongs();

        /// <summary>
        /// Adds a song to the library from a file path
        /// </summary>
        /// <param name="filePath">Path to the audio file</param>
        /// <returns>The added song</returns>
        Task<LocalSong> AddSongAsync(string filePath);

        /// <summary>
        /// Removes a song from the library
        /// </summary>
        /// <param name="songId">Song ID to remove</param>
        Task RemoveSongAsync(string songId);

        /// <summary>
        /// Gets a song by ID
        /// </summary>
        LocalSong? GetSong(string songId);

        /// <summary>
        /// Updates a song's information
        /// </summary>
        Task UpdateSongAsync(LocalSong song);

        /// <summary>
        /// Saves the library to disk
        /// </summary>
        Task SaveLibraryAsync();

        /// <summary>
        /// Loads the library from disk
        /// </summary>
        Task LoadLibraryAsync();

        /// <summary>
        /// Increments the play count for a song
        /// </summary>
        Task RecordPlayAsync(string songId);
    }
}
