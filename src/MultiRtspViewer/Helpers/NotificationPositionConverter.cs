using System;
using System.Globalization;
using System.Windows.Data;

namespace MultiRtspViewer.Helpers
{
    public class NotificationPositionConverter : IValueConverter
    {
        public double Offset { get; set; } = 0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width)
            {
                return width + Offset;
            }
            return Offset;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
