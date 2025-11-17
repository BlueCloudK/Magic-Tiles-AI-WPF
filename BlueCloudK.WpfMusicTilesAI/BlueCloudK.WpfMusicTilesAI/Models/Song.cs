namespace BlueCloudK.WpfMusicTilesAI.Models
{
    /// <summary>
    /// Represents a song that can be played
    /// </summary>
    public class Song
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Song title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Artist name
        /// </summary>
        public string Artist { get; set; } = string.Empty;

        /// <summary>
        /// Description that will be sent to AI for beat map generation
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Local file path or URL to the audio file
        /// </summary>
        public string Url { get; set; } = string.Empty;
    }
}
