using BlueCloudK.WpfMusicTilesAI.Models;
using BlueCloudK.WpfMusicTilesAI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.ViewModels
{
    /// <summary>
    /// ViewModel for the music library view
    /// </summary>
    public partial class LibraryViewModel : ObservableObject
    {
        private readonly IMusicLibraryService _libraryService;
        private readonly IBeatMapCacheService _beatMapCache;
        private readonly IGeminiService? _geminiService;
        private readonly IAudioAnalysisService? _audioAnalysisService;

        [ObservableProperty]
        private ObservableCollection<LocalSong> _songs = new();

        [ObservableProperty]
        private LocalSong? _selectedSong;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _loadingMessage = "";

        [ObservableProperty]
        private string? _errorMessage;

        [ObservableProperty]
        private bool _useAudioAnalysis = true; // Default to audio analysis if available

        public event Action<LocalSong>? OnPlaySong;

        public LibraryViewModel(
            IMusicLibraryService libraryService,
            IBeatMapCacheService beatMapCache,
            IGeminiService? geminiService = null,
            IAudioAnalysisService? audioAnalysisService = null)
        {
            _libraryService = libraryService ?? throw new ArgumentNullException(nameof(libraryService));
            _beatMapCache = beatMapCache ?? throw new ArgumentNullException(nameof(beatMapCache));
            _geminiService = geminiService;
            _audioAnalysisService = audioAnalysisService;

            // Check if audio analysis is available
            if (_audioAnalysisService != null && !_audioAnalysisService.IsAvailable())
            {
                _audioAnalysisService = null;
                UseAudioAnalysis = false;
            }
        }

        /// <summary>
        /// Loads the music library from disk
        /// </summary>
        public async Task LoadLibraryAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading your music library...";

                await _libraryService.LoadLibraryAsync();
                var songs = _libraryService.GetAllSongs();

                Songs.Clear();
                foreach (var song in songs)
                {
                    Songs.Add(song);
                }

                ErrorMessage = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load library: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Opens a file dialog to add music files to the library
        /// </summary>
        [RelayCommand]
        private async Task AddMusicAsync()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Audio Files (*.mp3;*.wav)|*.mp3;*.wav|All Files (*.*)|*.*",
                    Multiselect = true,
                    Title = "Select Music Files"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    IsLoading = true;
                    ErrorMessage = null;

                    foreach (var filePath in openFileDialog.FileNames)
                    {
                        LoadingMessage = $"Adding {Path.GetFileName(filePath)}...";

                        var song = await _libraryService.AddSongAsync(filePath);
                        Songs.Add(song);
                    }

                    LoadingMessage = "Music added successfully!";
                    await Task.Delay(1000); // Show success message briefly
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to add music: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Plays the selected song
        /// </summary>
        [RelayCommand]
        private async Task PlaySongAsync(LocalSong? song)
        {
            if (song == null) return;

            try
            {
                ErrorMessage = null;

                // Check if beat map exists
                if (!song.HasBeatMap)
                {
                    IsLoading = true;
                    BeatMap beatMap;

                    // Try audio analysis first if available and enabled
                    if (UseAudioAnalysis && _audioAnalysisService != null)
                    {
                        try
                        {
                            LoadingMessage = $"Analyzing audio ({song.SelectedDifficulty}): {song.Title}...";
                            beatMap = await _audioAnalysisService.AnalyzeAudioAsync(song.FilePath, song.Title, song.SelectedDifficulty);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Audio analysis failed: {ex.Message}");

                            // Fallback to AI generation if available
                            if (_geminiService != null)
                            {
                                LoadingMessage = $"Audio analysis failed, using AI generation...";
                                var description = $"A song titled {song.Title} by {song.Artist}";
                                beatMap = await _geminiService.GenerateBeatMapAsync(description);
                            }
                            else
                            {
                                ErrorMessage = $"Audio analysis failed and AI generation not available: {ex.Message}";
                                return;
                            }
                        }
                    }
                    else if (_geminiService != null)
                    {
                        // Use AI generation
                        LoadingMessage = $"Generating beat map for {song.Title}...";
                        var description = $"A song titled {song.Title} by {song.Artist}";
                        beatMap = await _geminiService.GenerateBeatMapAsync(description);
                    }
                    else
                    {
                        ErrorMessage = "Cannot generate beat map: No analysis method available";
                        return;
                    }

                    // Save to cache
                    var beatMapPath = await _beatMapCache.SaveBeatMapAsync(song.Id, beatMap);
                    song.BeatMapPath = beatMapPath;
                    song.HasBeatMap = true;

                    await _libraryService.UpdateSongAsync(song);

                    LoadingMessage = "Beat map created! Starting game...";
                    await Task.Delay(500);
                }

                // Record play
                await _libraryService.RecordPlayAsync(song.Id);

                // Note: No need to manually update Songs collection since LocalSong is now ObservableObject

                // Notify to start game
                OnPlaySong?.Invoke(song);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to play song: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Regenerates the beat map for a song
        /// </summary>
        [RelayCommand]
        private async Task RegenerateBeatMapAsync(LocalSong? song)
        {
            if (song == null) return;

            try
            {
                IsLoading = true;
                ErrorMessage = null;
                BeatMap beatMap;

                // Use audio analysis if available and enabled
                if (UseAudioAnalysis && _audioAnalysisService != null)
                {
                    try
                    {
                        LoadingMessage = $"Analyzing audio ({song.SelectedDifficulty}): {song.Title}...";
                        beatMap = await _audioAnalysisService.AnalyzeAudioAsync(song.FilePath, song.Title, song.SelectedDifficulty);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Audio analysis failed: {ex.Message}");

                        // Fallback to AI generation if available
                        if (_geminiService != null)
                        {
                            LoadingMessage = $"Audio analysis failed, using AI generation...";
                            var description = $"A song titled {song.Title} by {song.Artist}";
                            beatMap = await _geminiService.GenerateBeatMapAsync(description);
                        }
                        else
                        {
                            ErrorMessage = $"Audio analysis failed and AI generation not available: {ex.Message}";
                            return;
                        }
                    }
                }
                else if (_geminiService != null)
                {
                    // Use AI generation
                    LoadingMessage = $"Regenerating beat map for {song.Title}...";
                    var description = $"A song titled {song.Title} by {song.Artist}";
                    beatMap = await _geminiService.GenerateBeatMapAsync(description);
                }
                else
                {
                    ErrorMessage = "Cannot regenerate beat map: No analysis method available";
                    return;
                }

                // Save to cache (overwrites existing)
                var beatMapPath = await _beatMapCache.SaveBeatMapAsync(song.Id, beatMap);
                song.BeatMapPath = beatMapPath;
                song.HasBeatMap = true;

                await _libraryService.UpdateSongAsync(song);

                // Note: No need to manually update Songs collection since LocalSong is now ObservableObject

                LoadingMessage = "Beat map regenerated successfully!";
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to regenerate beat map: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Removes a song from the library
        /// </summary>
        [RelayCommand]
        private async Task RemoveSongAsync(LocalSong? song)
        {
            if (song == null) return;

            try
            {
                await _libraryService.RemoveSongAsync(song.Id);
                Songs.Remove(song);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to remove song: {ex.Message}";
            }
        }
    }
}
