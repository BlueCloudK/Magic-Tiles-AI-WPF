using BlueCloudK.WpfMusicTilesAI.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Service for analyzing audio files to automatically create beat maps
    /// Uses Python script with librosa for audio analysis
    /// </summary>
    public class AudioAnalysisService : IAudioAnalysisService
    {
        private readonly string _pythonScriptPath;
        private readonly string _pythonExecutable;

        public AudioAnalysisService()
        {
            // Path to Python script (relative to project root)
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
            _pythonScriptPath = Path.Combine(projectRoot, "BeatAnalysis", "analyze_audio.py");

            // Try to find Python executable
            _pythonExecutable = FindPythonExecutable();
        }

        /// <summary>
        /// Analyzes an audio file and generates a beat map automatically
        /// </summary>
        public async Task<BeatMap> AnalyzeAudioAsync(string audioFilePath, string songTitle)
        {
            if (!File.Exists(audioFilePath))
                throw new FileNotFoundException("Audio file not found", audioFilePath);

            if (!File.Exists(_pythonScriptPath))
                throw new FileNotFoundException($"Python script not found at: {_pythonScriptPath}");

            if (string.IsNullOrEmpty(_pythonExecutable))
                throw new InvalidOperationException("Python not found. Please install Python and librosa.");

            // Create temp file for output
            var tempOutputPath = Path.Combine(Path.GetTempPath(), $"beatmap_{Guid.NewGuid()}.json");

            try
            {
                // Run Python script
                var processInfo = new ProcessStartInfo
                {
                    FileName = _pythonExecutable,
                    Arguments = $"\"{_pythonScriptPath}\" \"{audioFilePath}\" \"{tempOutputPath}\" \"{songTitle}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processInfo);
                if (process == null)
                    throw new InvalidOperationException("Failed to start Python process");

                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"Python script failed: {error}");
                }

                System.Diagnostics.Debug.WriteLine($"Python output: {output}");

                // Wait a bit for file system to flush
                await Task.Delay(500);

                // Read generated beat map
                if (!File.Exists(tempOutputPath))
                    throw new FileNotFoundException("Beat map output file not created");

                // Validate file is complete before reading
                var fileInfo = new FileInfo(tempOutputPath);
                System.Diagnostics.Debug.WriteLine($"Beat map file size: {fileInfo.Length} bytes");

                if (fileInfo.Length == 0)
                    throw new InvalidOperationException("Beat map file is empty");

                var json = await File.ReadAllTextAsync(tempOutputPath);
                System.Diagnostics.Debug.WriteLine($"JSON length: {json.Length} characters");
                System.Diagnostics.Debug.WriteLine($"JSON preview (first 200 chars): {json.Substring(0, Math.Min(200, json.Length))}");
                System.Diagnostics.Debug.WriteLine($"JSON preview (last 200 chars): {json.Substring(Math.Max(0, json.Length - 200))}");

                BeatMap? beatMap;
                try
                {
                    beatMap = JsonConvert.DeserializeObject<BeatMap>(json);
                }
                catch (Exception ex)
                {
                    // Save failed JSON to temp file for debugging
                    var errorJsonPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"failed_beatmap_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                    await File.WriteAllTextAsync(errorJsonPath, json);
                    System.Diagnostics.Debug.WriteLine($"Failed JSON saved to: {errorJsonPath}");

                    // Try to extract path from JsonReaderException if available
                    var pathInfo = "";
                    if (ex is JsonReaderException jre)
                    {
                        pathInfo = $" at line {jre.LineNumber}, position {jre.LinePosition}, path '{jre.Path}'";
                    }

                    throw new InvalidOperationException($"JSON deserialization failed{pathInfo}: {ex.Message}\nJSON saved to: {errorJsonPath}", ex);
                }

                if (beatMap == null)
                    throw new InvalidOperationException("Failed to deserialize beat map");

                return beatMap;
            }
            finally
            {
                // Clean up temp file
                if (File.Exists(tempOutputPath))
                {
                    try { File.Delete(tempOutputPath); }
                    catch { /* Ignore cleanup errors */ }
                }
            }
        }

        /// <summary>
        /// Checks if audio analysis is available (Python + librosa installed)
        /// </summary>
        public bool IsAvailable()
        {
            if (string.IsNullOrEmpty(_pythonExecutable) || !File.Exists(_pythonScriptPath))
                return false;

            try
            {
                // Quick test: try to run Python
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = _pythonExecutable,
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                process?.WaitForExit(5000);
                return process?.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private string FindPythonExecutable()
        {
            // Try common Python executables
            var pythonNames = new[] { "python", "python3", "py" };

            foreach (var name in pythonNames)
            {
                try
                {
                    var process = Process.Start(new ProcessStartInfo
                    {
                        FileName = name,
                        Arguments = "--version",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });

                    if (process != null)
                    {
                        process.WaitForExit(5000);
                        if (process.ExitCode == 0)
                        {
                            return name;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            return string.Empty;
        }
    }
}
