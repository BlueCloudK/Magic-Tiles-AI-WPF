using BlueCloudK.WpfMusicTilesAI.Models;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Service for analyzing audio files to automatically generate beat maps
    /// </summary>
    public interface IAudioAnalysisService
    {
        /// <summary>
        /// Analyzes an audio file and generates a beat map based on detected beats
        /// </summary>
        /// <param name="audioFilePath">Path to the audio file (.mp3, .wav)</param>
        /// <param name="songTitle">Title of the song</param>
        /// <returns>Generated beat map</returns>
        Task<BeatMap> AnalyzeAudioAsync(string audioFilePath, string songTitle);

        /// <summary>
        /// Checks if audio analysis is available (Python + librosa installed)
        /// </summary>
        bool IsAvailable();
    }
}
