using BlueCloudK.WpfMusicTilesAI.ViewModels;
using System.Windows;

namespace BlueCloudK.WpfMusicTilesAI.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Execute SaveCommand and wait for it to complete before closing
            await _viewModel.SaveCommand.ExecuteAsync(null);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
