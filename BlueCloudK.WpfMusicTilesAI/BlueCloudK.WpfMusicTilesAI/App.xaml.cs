using BlueCloudK.WpfMusicTilesAI.Services;
using BlueCloudK.WpfMusicTilesAI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Windows;

namespace BlueCloudK.WpfMusicTilesAI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        public IServiceProvider Services => _serviceProvider
            ?? throw new InvalidOperationException("Service provider not initialized");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Setup Dependency Injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            // Show main window
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Get OAuth credentials from configuration
            var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")
                          ?? ConfigurationManager.AppSettings["GOOGLE_CLIENT_ID"];

            var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")
                              ?? ConfigurationManager.AppSettings["GOOGLE_CLIENT_SECRET"];

            // DEBUG: Log OAuth configuration
            System.Diagnostics.Debug.WriteLine($"=== App ConfigureServices ===");
            System.Diagnostics.Debug.WriteLine($"Client ID from config: '{clientId ?? "NULL"}'");
            System.Diagnostics.Debug.WriteLine($"Client Secret from config: '{(string.IsNullOrEmpty(clientSecret) ? "NULL/EMPTY" : "***SET***")}'");

            // Register services
            // OAuth is now required for Gemini API
            // Check if credentials are configured (not placeholders)
            bool hasValidOAuthCredentials = !string.IsNullOrEmpty(clientId)
                && !string.IsNullOrEmpty(clientSecret)
                && !clientId.Contains("YOUR_")
                && !clientSecret.Contains("YOUR_");

            System.Diagnostics.Debug.WriteLine($"Has valid OAuth credentials: {hasValidOAuthCredentials}");

            if (hasValidOAuthCredentials)
            {
                System.Diagnostics.Debug.WriteLine("Registering GoogleAuthService and GeminiService");
                services.AddSingleton<IGoogleAuthService>(sp => new GoogleAuthService(clientId!, clientSecret!));

                // Register Gemini service using OAuth authentication
                services.AddSingleton<IGeminiService>(sp =>
                {
                    var authService = sp.GetRequiredService<IGoogleAuthService>();
                    return new GeminiService(authService);
                });

                // Register LoginViewModel only when OAuth is configured
                services.AddTransient<LoginViewModel>();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OAuth not configured - services not registered");
            }
            System.Diagnostics.Debug.WriteLine("============================");

            services.AddSingleton<IAudioService, AudioService>();

            // Register ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<StartViewModel>();
            services.AddTransient<GameViewModel>();

            // Register Main Window
            services.AddSingleton<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
