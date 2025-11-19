using BlueCloudK.WpfMusicTilesAI.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BlueCloudK.WpfMusicTilesAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;
        private readonly StartViewModel _startViewModel;
        private GameViewModel? _gameViewModel;

        public MainWindow(MainViewModel mainViewModel, StartViewModel startViewModel)
        {
            System.Diagnostics.Debug.WriteLine("=== MainWindow Constructor ===");
            System.Diagnostics.Debug.WriteLine("Calling InitializeComponent...");
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("InitializeComponent completed");

            _mainViewModel = mainViewModel;
            _startViewModel = startViewModel;

            System.Diagnostics.Debug.WriteLine("Setting DataContext...");
            DataContext = _mainViewModel;
            StartView.DataContext = _startViewModel;
            System.Diagnostics.Debug.WriteLine("DataContext set");

            // Wire up StartView loaded event to handle library playback
            StartView.Loaded += (s, e) =>
            {
                // Wire up library playback event
                if (StartView is Views.StartView startView)
                {
                    startView.PlayLocalSong += HandlePlayLocalSong;
                }
            };

            // Wire up events
            _startViewModel.OnStartGame += async (description) =>
            {
                System.Diagnostics.Debug.WriteLine($"=== OnStartGame event triggered: {description} ===");
                await _mainViewModel.StartGameCommand.ExecuteAsync(description);

                System.Diagnostics.Debug.WriteLine($"CurrentState: {_mainViewModel.CurrentState}, BeatMap: {(_mainViewModel.CurrentBeatMap != null ? "exists" : "null")}");

                // Once game starts, setup GameView
                if (_mainViewModel.CurrentState == Models.GameState.Playing && _mainViewModel.CurrentBeatMap != null)
                {
                    System.Diagnostics.Debug.WriteLine("Creating GameViewModel...");
                    var app = (App)Application.Current;
                    var gameViewModel = new GameViewModel(
                        app.Services.GetService(typeof(Services.IAudioService)) as Services.IAudioService
                            ?? throw new System.Exception("AudioService not found"),
                        app.Services.GetService(typeof(Services.ISettingsService)) as Services.ISettingsService
                            ?? throw new System.Exception("SettingsService not found"));

                    gameViewModel.Initialize(_mainViewModel.CurrentBeatMap, _mainViewModel.CurrentSong!);
                    gameViewModel.OnGameEnd += () =>
                    {
                        _mainViewModel.EndGameCommand.Execute(null);
                    };

                    GameView.DataContext = gameViewModel;
                    _gameViewModel = gameViewModel;
                    System.Diagnostics.Debug.WriteLine("GameViewModel initialized");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("WARNING: Game state or beatmap invalid!");
                }
            };

            _mainViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.CurrentState))
                {
                    if (_mainViewModel.CurrentState == Models.GameState.Start)
                    {
                        _gameViewModel?.Dispose();
                        _gameViewModel = null;
                    }
                }
            };

            System.Diagnostics.Debug.WriteLine("MainWindow constructor completed successfully");
            System.Diagnostics.Debug.WriteLine("==============================");
        }

        private async void HandlePlayLocalSong(Models.LocalSong song)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== HandlePlayLocalSong called: {song.Title} ===");

                // Get services
                var app = (App)Application.Current;
                var beatMapCache = app.Services.GetService(typeof(Services.IBeatMapCacheService)) as Services.IBeatMapCacheService;

                if (beatMapCache == null || string.IsNullOrEmpty(song.BeatMapPath))
                {
                    System.Diagnostics.Debug.WriteLine("Cannot play song: beat map not found");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Loading beat map from: {song.BeatMapPath}");

                // Load beat map from cache
                var beatMap = await beatMapCache.LoadBeatMapAsync(song.BeatMapPath);
                if (beatMap == null)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to load beat map from cache");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Beat map loaded: {beatMap.Notes.Count} notes");

                // Set current beat map and song
                _mainViewModel.CurrentBeatMap = beatMap;
                _mainViewModel.CurrentSong = song;

                System.Diagnostics.Debug.WriteLine($"Setting CurrentState to Playing...");
                _mainViewModel.CurrentState = Models.GameState.Playing;
                System.Diagnostics.Debug.WriteLine($"CurrentState is now: {_mainViewModel.CurrentState}");

                // Setup GameView
                System.Diagnostics.Debug.WriteLine("Creating GameViewModel...");
                var gameViewModel = new GameViewModel(
                    app.Services.GetService(typeof(Services.IAudioService)) as Services.IAudioService
                        ?? throw new System.Exception("AudioService not found"),
                    app.Services.GetService(typeof(Services.ISettingsService)) as Services.ISettingsService
                        ?? throw new System.Exception("SettingsService not found"));

                System.Diagnostics.Debug.WriteLine("Calling GameViewModel.Initialize...");
                gameViewModel.Initialize(beatMap, song);
                System.Diagnostics.Debug.WriteLine("GameViewModel.Initialize completed");

                gameViewModel.OnGameEnd += () =>
                {
                    System.Diagnostics.Debug.WriteLine("OnGameEnd event triggered in HandlePlayLocalSong");
                    _mainViewModel.EndGameCommand.Execute(null);
                };

                System.Diagnostics.Debug.WriteLine("Setting GameView.DataContext...");
                GameView.DataContext = gameViewModel;
                _gameViewModel = gameViewModel;
                System.Diagnostics.Debug.WriteLine($"GameView.DataContext set, IsFocused: {GameView.IsFocused}");
                System.Diagnostics.Debug.WriteLine("=== HandlePlayLocalSong completed ===");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error playing local song: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Forward key events to GameViewModel when in Playing state
            if (_mainViewModel.CurrentState == Models.GameState.Playing && _gameViewModel != null)
            {
                int lane = e.Key switch
                {
                    Key.D => 1,
                    Key.F => 2,
                    Key.J => 3,
                    Key.K => 4,
                    _ => 0
                };

                if (lane > 0)
                {
                    _gameViewModel.HandleKeyPress(lane);
                    e.Handled = true;
                }
            }
        }
    }
}