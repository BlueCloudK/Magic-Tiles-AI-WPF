using BlueCloudK.WpfMusicTilesAI.Models;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Interface for Gemini AI service to generate beat maps
    /// </summary>
    public interface IGeminiService
    {
        /// <summary>
        /// Generates a beat map based on the provided song description
        /// </summary>
        /// <param name="description">Description of the song (genre, mood, tempo, etc.)</param>
        /// <returns>Generated beat map with notes and metadata</returns>
        Task<BeatMap> GenerateBeatMapAsync(string description);
    }
}
