using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.ViewModels
{
    /// <summary>
    /// View model for the start screen
    /// </summary>
    public partial class StartViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _songDescription = string.Empty;

        [ObservableProperty]
        private float _volume = 0.3f;

        [ObservableProperty]
        private string? _errorMessage;

        public event Func<string, Task>? OnStartGame;

        [RelayCommand(CanExecute = nameof(CanStartGame))]
        private async Task StartGameAsync()
        {
            if (OnStartGame != null)
            {
                await OnStartGame(SongDescription);
            }
        }

        private bool CanStartGame()
        {
            return !string.IsNullOrWhiteSpace(SongDescription);
        }

        partial void OnSongDescriptionChanged(string value)
        {
            StartGameCommand.NotifyCanExecuteChanged();
        }
    }
}
