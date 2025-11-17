using BlueCloudK.WpfMusicTilesAI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.ViewModels
{
    /// <summary>
    /// ViewModel for settings dialog
    /// </summary>
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private float _volume;

        [ObservableProperty]
        private string _selectedTheme;

        [ObservableProperty]
        private float _speed;

        [ObservableProperty]
        private bool _showFPS;

        [ObservableProperty]
        private bool _enableEffects;

        public List<string> AvailableThemes { get; } = new List<string>
        {
            "Red",
            "Blue",
            "Green",
            "Purple",
            "Orange",
            "Pink",
            "Rainbow"
        };

        public event Action? OnSettingsChanged;

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

            // Load current settings
            _volume = _settingsService.Settings.Volume;
            _selectedTheme = _settingsService.Settings.NoteTheme;
            _speed = _settingsService.Settings.Speed;
            _showFPS = _settingsService.Settings.ShowFPS;
            _enableEffects = _settingsService.Settings.EnableEffects;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                _settingsService.Settings.Volume = Volume;
                _settingsService.Settings.NoteTheme = SelectedTheme;
                _settingsService.Settings.Speed = Speed;
                _settingsService.Settings.ShowFPS = ShowFPS;
                _settingsService.Settings.EnableEffects = EnableEffects;

                await _settingsService.SaveSettingsAsync();

                // Notify that settings changed
                OnSettingsChanged?.Invoke();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets to default settings
        /// </summary>
        [RelayCommand]
        private async Task ResetToDefaultsAsync()
        {
            await _settingsService.ResetToDefaultsAsync();

            // Reload UI
            Volume = _settingsService.Settings.Volume;
            SelectedTheme = _settingsService.Settings.NoteTheme;
            Speed = _settingsService.Settings.Speed;
            ShowFPS = _settingsService.Settings.ShowFPS;
            EnableEffects = _settingsService.Settings.EnableEffects;

            OnSettingsChanged?.Invoke();
        }
    }
}
