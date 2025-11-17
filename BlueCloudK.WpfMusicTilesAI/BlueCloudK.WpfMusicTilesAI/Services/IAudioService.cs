using System;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Interface for audio playback service
    /// </summary>
    public interface IAudioService : IDisposable
    {
        /// <summary>
        /// Loads an audio file from the specified path
        /// </summary>
        void Load(string filePath);

        /// <summary>
        /// Starts or resumes audio playback
        /// </summary>
        void Play();

        /// <summary>
        /// Pauses audio playback
        /// </summary>
        void Pause();

        /// <summary>
        /// Stops audio playback and resets position
        /// </summary>
        void Stop();

        /// <summary>
        /// Gets the current playback position in seconds
        /// </summary>
        double CurrentPosition { get; }

        /// <summary>
        /// Gets the total duration of the loaded audio in seconds
        /// </summary>
        double TotalDuration { get; }

        /// <summary>
        /// Gets or sets the volume (0.0 to 1.0)
        /// </summary>
        float Volume { get; set; }

        /// <summary>
        /// Gets whether audio is currently playing
        /// </summary>
        bool IsPlaying { get; }
    }
}
