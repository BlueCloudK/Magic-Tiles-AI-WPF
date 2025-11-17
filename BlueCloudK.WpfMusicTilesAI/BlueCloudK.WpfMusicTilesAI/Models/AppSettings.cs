namespace BlueCloudK.WpfMusicTilesAI.Models
{
    /// <summary>
    /// Application settings model
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Master volume (0.0 - 1.0)
        /// </summary>
        public float Volume { get; set; } = 0.5f;

        /// <summary>
        /// Note theme color
        /// </summary>
        public string NoteTheme { get; set; } = "Red"; // Red, Blue, Green, Purple, Rainbow

        /// <summary>
        /// Game speed multiplier
        /// </summary>
        public float Speed { get; set; } = 1.0f;

        /// <summary>
        /// Show FPS counter
        /// </summary>
        public bool ShowFPS { get; set; } = false;

        /// <summary>
        /// Enable visual effects
        /// </summary>
        public bool EnableEffects { get; set; } = true;

        /// <summary>
        /// Version for settings migration
        /// </summary>
        public int Version { get; set; } = 1;
    }

    /// <summary>
    /// Available note themes
    /// </summary>
    public enum NoteTheme
    {
        Red,
        Blue,
        Green,
        Purple,
        Orange,
        Pink,
        Rainbow
    }
}
