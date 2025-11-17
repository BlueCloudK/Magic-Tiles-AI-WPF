using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueCloudK.WpfMusicTilesAI.Helpers
{
    /// <summary>
    /// Converts lane number (1-4) to X position (0, 100, 200, 300)
    /// </summary>
    public class LaneToXConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int lane)
            {
                return (lane - 1) * 100 + 5; // 5px padding from lane edge
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
