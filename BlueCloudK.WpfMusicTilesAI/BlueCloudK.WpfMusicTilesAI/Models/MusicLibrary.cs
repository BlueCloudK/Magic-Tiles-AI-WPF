using System;
using System.Collections.Generic;

namespace BlueCloudK.WpfMusicTilesAI.Models
{
    /// <summary>
    /// Represents the user's music library
    /// </summary>
    public class MusicLibrary
    {
        /// <summary>
        /// List of songs in the library
        /// </summary>
        public List<LocalSong> Songs { get; set; } = new List<LocalSong>();

        /// <summary>
        /// Last time the library was updated
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// Version of the library format (for future compatibility)
        /// </summary>
        public int Version { get; set; } = 1;
    }
}
