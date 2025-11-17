using BlueCloudK.WpfMusicTilesAI.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueCloudK.WpfMusicTilesAI.Helpers
{
    /// <summary>
    /// Converts GameState to Visibility based on parameter
    /// </summary>
    public class GameStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GameState currentState && parameter is string targetState)
            {
                if (Enum.TryParse<GameState>(targetState, out var target))
                {
                    return currentState == target ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
