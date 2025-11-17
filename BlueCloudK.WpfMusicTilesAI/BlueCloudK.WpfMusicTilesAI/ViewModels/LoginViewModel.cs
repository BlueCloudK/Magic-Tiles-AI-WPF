using BlueCloudK.WpfMusicTilesAI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.ViewModels
{
    /// <summary>
    /// View model for the login screen
    /// </summary>
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IGoogleAuthService _authService;

        [ObservableProperty]
        private string? _errorMessage;

        [ObservableProperty]
        private bool _isAuthenticating;

        public event Func<Task>? OnAuthenticationSuccess;

        public LoginViewModel(IGoogleAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [RelayCommand]
        private async Task SignInAsync()
        {
            try
            {
                ErrorMessage = null;
                IsAuthenticating = true;

                await _authService.AuthenticateAsync();

                if (_authService.IsAuthenticated)
                {
                    if (OnAuthenticationSuccess != null)
                    {
                        await OnAuthenticationSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to sign in: {ex.Message}";
            }
            finally
            {
                IsAuthenticating = false;
            }
        }
    }
}
