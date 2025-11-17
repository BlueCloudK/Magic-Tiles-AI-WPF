using CommunityToolkit.Mvvm.ComponentModel;

namespace BlueCloudK.WpfMusicTilesAI.Models
{
    /// <summary>
    /// Represents a single note/tile in the game
    /// </summary>
    public partial class Note : ObservableObject
    {
        /// <summary>
        /// The exact second when the note should be hit
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// The lane number (1-4) where the note appears
        /// </summary>
        public int Lane { get; set; }

        /// <summary>
        /// Unique identifier for the note
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Optional duration in seconds for long/sustain notes
        /// If null, it's a standard tap note
        /// </summary>
        public double? Duration { get; set; }

        /// <summary>
        /// Current state of the note during gameplay
        /// </summary>
        [ObservableProperty]
        private NoteState _state = NoteState.Active;

        /// <summary>
        /// Y position of the note on screen (updated during game loop)
        /// </summary>
        [ObservableProperty]
        private double _y;

        /// <summary>
        /// Time when the note was successfully hit
        /// </summary>
        public double? HitTime { get; set; }

        /// <summary>
        /// Progress of holding a long note (0 to 1)
        /// </summary>
        [ObservableProperty]
        private double _holdProgress;
    }

    /// <summary>
    /// Possible states of a note during gameplay
    /// </summary>
    public enum NoteState
    {
        Active,   // Note is falling down
        Hit,      // Note was successfully hit
        Missed,   // Note was missed
        Holding,  // Long note is being held
        Held      // Long note was successfully held to completion
    }
}
