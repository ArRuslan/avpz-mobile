using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace UniMobileProject.src.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BookingStatus status)
            {
                Color GetResourceColor(string key)
                {
                    return (Color)Application.Current.Resources[key];
                }

                return status switch
                {
                    BookingStatus.PENDING => GetResourceColor("PendingStatusColor"),
                    BookingStatus.ACTIVE => GetResourceColor("ActiveStatusColor"),
                    BookingStatus.CANCELLED => GetResourceColor("CancelledStatusColor"),
                    BookingStatus.EXPIRED => GetResourceColor("ExpiredStatusColor"),
                    BookingStatus.ALL => GetResourceColor("AllStatusColor"),
                    _ => GetResourceColor("AllStatusColor")
                };
            }

            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
