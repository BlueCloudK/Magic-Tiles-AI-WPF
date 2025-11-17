using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace BlueCloudK.WpfMusicTilesAI.Helpers
{
    /// <summary>
    /// Helper class for loading and managing images
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Gets the default album art placeholder image path
        /// </summary>
        public static string AlbumPlaceholderPath => "/Assets/Images/album-placeholder.png";

        /// <summary>
        /// Gets the application logo image path
        /// </summary>
        public static string AppLogoPath => "/Assets/Logos/app-logo.png";

        /// <summary>
        /// Loads an image from a file path, returns placeholder if file doesn't exist
        /// </summary>
        public static BitmapImage? LoadImageOrDefault(string? imagePath, string defaultPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
                else if (!string.IsNullOrEmpty(defaultPath))
                {
                    // Try to load default from resources
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(defaultPath, UriKind.RelativeOrAbsolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load image: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Checks if an image file exists at the given path
        /// </summary>
        public static bool ImageExists(string? imagePath)
        {
            return !string.IsNullOrEmpty(imagePath) && File.Exists(imagePath);
        }

        /// <summary>
        /// Gets the appropriate image source (file path or resource path)
        /// </summary>
        public static string GetImageSource(string? customPath, string defaultResourcePath)
        {
            if (!string.IsNullOrEmpty(customPath) && File.Exists(customPath))
            {
                return customPath;
            }
            return defaultResourcePath;
        }
    }
}
