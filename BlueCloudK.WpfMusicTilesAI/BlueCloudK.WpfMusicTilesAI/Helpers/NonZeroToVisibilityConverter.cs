using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueCloudK.WpfMusicTilesAI.Helpers
{
    /// <summary>
    /// Converts a numeric value to Visibility (non-zero = Visible, 0 = Collapsed)
    /// Used for showing content when a value exists
    /// </summary>
    public class NonZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
