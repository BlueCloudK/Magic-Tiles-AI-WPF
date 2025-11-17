using BlueCloudK.WpfMusicTilesAI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlueCloudK.WpfMusicTilesAI.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("GameView loaded, setting focus...");
                var focused = Focus();
                System.Diagnostics.Debug.WriteLine($"Focus result: {focused}, IsFocused: {IsFocused}");
            };
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"KeyDown event: {e.Key}");

            if (DataContext is GameViewModel viewModel)
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
                    System.Diagnostics.Debug.WriteLine($"Handling key press for lane {lane}");
                    viewModel.HandleKeyPress(lane);
                    e.Handled = true;
                }
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Get services from app
            var app = (App)Application.Current;
            var settingsService = app.Services.GetService(typeof(Services.ISettingsService)) as Services.ISettingsService;

            if (settingsService != null)
            {
                // Create and show settings window
                var settingsViewModel = new SettingsViewModel(settingsService);
                var settingsWindow = new SettingsWindow(settingsViewModel)
                {
                    Owner = Window.GetWindow(this)
                };
                settingsWindow.ShowDialog();
            }
        }
    }
}
