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

            try
            {
                System.Diagnostics.Debug.WriteLine("=== App OnStartup ===");

                // Setup Dependency Injection
                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                System.Diagnostics.Debug.WriteLine("ServiceProvider built successfully");

                // Show main window
                System.Diagnostics.Debug.WriteLine("Creating MainWindow...");
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

                System.Diagnostics.Debug.WriteLine("Setting MainWindow property...");
                MainWindow = mainWindow;

                System.Diagnostics.Debug.WriteLine("Showing MainWindow...");
                mainWindow.Show();

                System.Diagnostics.Debug.WriteLine("MainWindow shown successfully");
                System.Diagnostics.Debug.WriteLine("=====================");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FATAL ERROR in OnStartup: {ex}");
                MessageBox.Show($"Failed to start application:\n\n{ex.Message}\n\nStack trace:\n{ex.StackTrace}",
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Get Gemini API Key from configuration
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                        ?? ConfigurationManager.AppSettings["GEMINI_API_KEY"];

            // Get Gemini Model from configuration (default: gemini-2.0-flash-exp)
            var model = Environment.GetEnvironmentVariable("GEMINI_MODEL")
                       ?? ConfigurationManager.AppSettings["GEMINI_MODEL"]
                       ?? "gemini-2.0-flash-exp";

            // DEBUG: Log API Key configuration
            System.Diagnostics.Debug.WriteLine($"=== App ConfigureServices ===");
            System.Diagnostics.Debug.WriteLine($"API Key from config: '{(string.IsNullOrEmpty(apiKey) ? "NULL/EMPTY" : "***SET***")}'");
            System.Diagnostics.Debug.WriteLine($"Gemini Model: {model}");

            // Register services
            // Check if API key is configured (not placeholders)
            bool hasValidApiKey = !string.IsNullOrEmpty(apiKey)
                && !apiKey.Contains("YOUR_")
                && !apiKey.Contains("API_KEY_HERE");

            System.Diagnostics.Debug.WriteLine($"Has valid API Key: {hasValidApiKey}");

            if (hasValidApiKey)
            {
                System.Diagnostics.Debug.WriteLine($"Registering GeminiService with model: {model}");
                // Register Gemini service with API Key and selected model
                services.AddSingleton<IGeminiService>(sp => new GeminiService(apiKey!, model));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("API Key not configured - GeminiService not registered");
            }
            System.Diagnostics.Debug.WriteLine("============================");

            services.AddSingleton<IAudioService, AudioService>();
            services.AddSingleton<IBeatMapCacheService, BeatMapCacheService>();
            services.AddSingleton<IMusicLibraryService, MusicLibraryService>();
            services.AddSingleton<IAudioAnalysisService, AudioAnalysisService>();

            // Settings Service
            var settingsService = new SettingsService();
            settingsService.LoadSettingsAsync().Wait(); // Load settings on startup
            services.AddSingleton<ISettingsService>(settingsService);

            // Register ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<StartViewModel>();
            services.AddTransient<GameViewModel>();
            services.AddTransient<LibraryViewModel>();
            services.AddTransient<SettingsViewModel>();

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
