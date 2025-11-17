using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueCloudK.WpfMusicTilesAI.Helpers
{
    /// <summary>
    /// Converts a numeric value to Visibility (0 = Visible, non-zero = Collapsed)
    /// Used for showing empty state when collection count is 0
    /// </summary>
    public class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
