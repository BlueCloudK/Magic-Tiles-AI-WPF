using BlueCloudK.WpfMusicTilesAI.Models;
using BlueCloudK.WpfMusicTilesAI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.ViewModels
{
    /// <summary>
    /// Main view model that manages the overall game state and navigation
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private readonly IGeminiService? _geminiService;
        private readonly IAudioService _audioService;

        [ObservableProperty]
        private GameState _currentState = GameState.Start;

        [ObservableProperty]
        private bool _showStartView = false;

        [ObservableProperty]
        private bool _showSetupInstructions = false;

        [ObservableProperty]
        private BeatMap? _currentBeatMap;

        [ObservableProperty]
        private Song? _currentSong;

        [ObservableProperty]
        private string? _errorMessage;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _loadingMessage = "Loading...";

        [ObservableProperty]
        private float _volume = 0.3f;

        public MainViewModel(IAudioService audioService, IGeminiService? geminiService = null)
        {
            _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
            _geminiService = geminiService;

            // DEBUG: Log startup state
            System.Diagnostics.Debug.WriteLine($"=== MainViewModel Constructor ===");
            System.Diagnostics.Debug.WriteLine($"GeminiService is null: {_geminiService == null}");

            // Determine which screen to show
            if (_geminiService == null)
            {
                // No API Key configured - show setup instructions
                System.Diagnostics.Debug.WriteLine("Showing Setup Instructions (API Key not configured)");
                ShowSetupInstructions = true;
                ShowStartView = false;
            }
            else
            {
                // API Key configured - show start view
                System.Diagnostics.Debug.WriteLine("Showing StartView (API Key configured)");
                ShowSetupInstructions = false;
                ShowStartView = CurrentState == GameState.Start;
            }

            System.Diagnostics.Debug.WriteLine($"ShowSetupInstructions: {ShowSetupInstructions}");
            System.Diagnostics.Debug.WriteLine($"ShowStartView: {ShowStartView}");
            System.Diagnostics.Debug.WriteLine("=================================");
        }

        /// <summary>
        /// Starts the game with the specified song description
        /// </summary>
        [RelayCommand]
        private async Task StartGameAsync(string songDescription)
        {
            try
            {
                if (_geminiService == null)
                {
                    ErrorMessage = "Gemini service is not configured. Please add your GEMINI_API_KEY to App.config";
                    return;
                }

                ErrorMessage = null;
                IsLoading = true;
                CurrentState = GameState.Loading;
                LoadingMessage = $"Composing beat map for: {songDescription}";

                // Generate beat map using AI
                var beatMap = await _geminiService.GenerateBeatMapAsync(songDescription);

                CurrentBeatMap = beatMap;
                CurrentSong = new Song
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = beatMap.Metadata.Title,
                    Description = songDescription,
                    Artist = "AI Generated"
                };

                CurrentState = GameState.Playing;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to generate beat map: {ex.Message}";
                CurrentState = GameState.Start;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Ends the current game and returns to start screen
        /// </summary>
        [RelayCommand]
        private void EndGame()
        {
            _audioService.Stop();
            CurrentState = GameState.Start;
            CurrentBeatMap = null;
            CurrentSong = null;
            ErrorMessage = null;
        }

        /// <summary>
        /// Updates the volume
        /// </summary>
        partial void OnVolumeChanged(float value)
        {
            _audioService.Volume = value;
        }

        /// <summary>
        /// Updates ShowStartView when CurrentState changes
        /// </summary>
        partial void OnCurrentStateChanged(GameState value)
        {
            ShowStartView = value == GameState.Start;
        }
    }
}
