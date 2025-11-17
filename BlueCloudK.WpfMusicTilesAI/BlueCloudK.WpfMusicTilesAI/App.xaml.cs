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
            // Get Gemini API Key from configuration
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                        ?? ConfigurationManager.AppSettings["GEMINI_API_KEY"];

            // DEBUG: Log API Key configuration
            System.Diagnostics.Debug.WriteLine($"=== App ConfigureServices ===");
            System.Diagnostics.Debug.WriteLine($"API Key from config: '{(string.IsNullOrEmpty(apiKey) ? "NULL/EMPTY" : "***SET***")}'");

            // Register services
            // Check if API key is configured (not placeholders)
            bool hasValidApiKey = !string.IsNullOrEmpty(apiKey)
                && !apiKey.Contains("YOUR_")
                && !apiKey.Contains("API_KEY_HERE");

            System.Diagnostics.Debug.WriteLine($"Has valid API Key: {hasValidApiKey}");

            if (hasValidApiKey)
            {
                System.Diagnostics.Debug.WriteLine("Registering GeminiService with API Key");
                // Register Gemini service with API Key
                services.AddSingleton<IGeminiService>(sp => new GeminiService(apiKey!));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("API Key not configured - GeminiService not registered");
            }
            System.Diagnostics.Debug.WriteLine("============================");

            services.AddSingleton<IAudioService, AudioService>();
            services.AddSingleton<IBeatMapCacheService, BeatMapCacheService>();
            services.AddSingleton<IMusicLibraryService, MusicLibraryService>();

            // Register ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<StartViewModel>();
            services.AddTransient<GameViewModel>();
            services.AddTransient<LibraryViewModel>();

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
