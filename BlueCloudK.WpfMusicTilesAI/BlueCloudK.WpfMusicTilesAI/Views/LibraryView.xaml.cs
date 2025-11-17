using BlueCloudK.WpfMusicTilesAI.Services;
using BlueCloudK.WpfMusicTilesAI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BlueCloudK.WpfMusicTilesAI.Views
{
    /// <summary>
    /// Interaction logic for LibraryView.xaml
    /// </summary>
    public partial class LibraryView : UserControl
    {
        public LibraryView()
        {
            InitializeComponent();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Get services from DI container
            var app = (App)Application.Current;
            var settingsService = app.Services.GetService(typeof(ISettingsService)) as ISettingsService;

            if (settingsService != null)
            {
                var settingsViewModel = new SettingsViewModel(settingsService);
                var settingsWindow = new SettingsWindow(settingsViewModel);
                settingsWindow.Owner = Window.GetWindow(this);

                var result = settingsWindow.ShowDialog();
                if (result == true)
                {
                    // Settings saved, apply them
                    // You can add event handling here if needed
                }
            }
        }
    }
}
