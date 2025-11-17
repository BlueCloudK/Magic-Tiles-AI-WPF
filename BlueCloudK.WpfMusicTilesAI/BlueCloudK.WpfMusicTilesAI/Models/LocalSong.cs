using System;

namespace BlueCloudK.WpfMusicTilesAI.Models
{
    /// <summary>
    /// Represents a song imported from local file system
    /// </summary>
    public class LocalSong : Song
    {
        /// <summary>
        /// Full path to the audio file on disk
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// File name without extension
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Date when the song was added to library
        /// </summary>
        public DateTime DateAdded { get; set; } = DateTime.Now;

        /// <summary>
        /// Whether a beat map has been generated for this song
        /// </summary>
        public bool HasBeatMap { get; set; }

        /// <summary>
        /// Path to the cached beat map JSON file
        /// </summary>
        public string? BeatMapPath { get; set; }

        /// <summary>
        /// Duration of the audio file in seconds (if available)
        /// </summary>
        public double? Duration { get; set; }

        /// <summary>
        /// Last time this song was played
        /// </summary>
        public DateTime? LastPlayed { get; set; }

        /// <summary>
        /// Number of times this song has been played
        /// </summary>
        public int PlayCount { get; set; }
    }
}
