using BlueCloudK.WpfMusicTilesAI.ViewModels;
using System.Windows;

namespace BlueCloudK.WpfMusicTilesAI.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
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
