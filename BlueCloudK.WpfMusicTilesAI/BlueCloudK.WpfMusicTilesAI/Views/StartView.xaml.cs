using BlueCloudK.WpfMusicTilesAI.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BlueCloudK.WpfMusicTilesAI.Views
{
    /// <summary>
    /// Interaction logic for StartView.xaml
    /// </summary>
    public partial class StartView : UserControl
    {
        private LibraryViewModel? _libraryViewModel;

        public event Action<Models.LocalSong>? PlayLocalSong;

        public StartView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Get LibraryViewModel from DI container and setup LibraryView
            var app = (App)Application.Current;
            _libraryViewModel = app.Services.GetService(typeof(LibraryViewModel)) as LibraryViewModel;

            if (_libraryViewModel != null)
            {
                LibraryViewControl.DataContext = _libraryViewModel;

                // Wire up the OnPlaySong event to start game with local song
                _libraryViewModel.OnPlaySong += HandlePlayLocalSong;

                // Load the library
                await _libraryViewModel.LoadLibraryAsync();
            }
        }

        private void HandlePlayLocalSong(Models.LocalSong song)
        {
            // Forward the event to MainWindow
            PlayLocalSong?.Invoke(song);
        }
    }
}
