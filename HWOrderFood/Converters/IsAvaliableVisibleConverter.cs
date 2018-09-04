using System;
using System.Globalization;
using Xamarin.Forms;

namespace HWOrderFood.Converters
{
    public class IsAvaliableVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
