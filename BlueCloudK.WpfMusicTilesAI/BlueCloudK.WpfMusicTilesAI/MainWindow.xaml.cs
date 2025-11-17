using BlueCloudK.WpfMusicTilesAI.ViewModels;
using System.Diagnostics;
using System.Windows;
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
            InitializeComponent();

            _mainViewModel = mainViewModel;
            _startViewModel = startViewModel;

            DataContext = _mainViewModel;
            StartView.DataContext = _startViewModel;

            // Wire up events
            _startViewModel.OnStartGame += async (description) =>
            {
                await _mainViewModel.StartGameCommand.ExecuteAsync(description);

                // Once game starts, setup GameView
                if (_mainViewModel.CurrentState == Models.GameState.Playing && _mainViewModel.CurrentBeatMap != null)
                {
                    var gameViewModel = new GameViewModel(
                        ((App)Application.Current).Services.GetService(typeof(Services.IAudioService)) as Services.IAudioService
                            ?? throw new System.Exception("AudioService not found"));

                    gameViewModel.Initialize(_mainViewModel.CurrentBeatMap, _mainViewModel.CurrentSong!);
                    gameViewModel.OnGameEnd += () =>
                    {
                        _mainViewModel.EndGameCommand.Execute(null);
                    };

                    GameView.DataContext = gameViewModel;
                    _gameViewModel = gameViewModel;
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
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}