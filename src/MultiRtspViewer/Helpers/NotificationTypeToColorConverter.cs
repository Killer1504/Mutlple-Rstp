using MultiRtspViewer.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiRtspViewer.Helpers
{
    public class NotificationTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NotificationType type)
            {
                return type switch
                {
                    NotificationType.Success => new SolidColorBrush(Color.FromRgb(34, 197, 94)), // Green
                    NotificationType.Warning => new SolidColorBrush(Color.FromRgb(251, 191, 36)), // Yellow
                    NotificationType.Error => new SolidColorBrush(Color.FromRgb(239, 68, 68)), // Red
                    _ => new SolidColorBrush(Color.FromRgb(34, 211, 238)), // Cyan (Info)
                };
            }
            return new SolidColorBrush(Color.FromRgb(34, 211, 238));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
