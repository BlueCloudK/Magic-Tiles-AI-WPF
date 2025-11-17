namespace BlueCloudK.WpfMusicTilesAI.Models
{
    /// <summary>
    /// Represents the current state of the game
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Start screen - user can select/input song description
        /// </summary>
        Start,

        /// <summary>
        /// Loading screen - AI is generating the beat map
        /// </summary>
        Loading,

        /// <summary>
        /// Game is actively being played
        /// </summary>
        Playing,

        /// <summary>
        /// Game has finished, showing results
        /// </summary>
        Finished
    }
}
