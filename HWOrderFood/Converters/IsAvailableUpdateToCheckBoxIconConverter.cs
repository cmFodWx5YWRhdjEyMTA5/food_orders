using System;
using System.Globalization;
using Xamarin.Forms;

namespace HWOrderFood.Converters
{
    public class IsAvailableUpdateToCheckBoxIconConverter : IValueConverter
    {
        private string _firstImageSource;
        private string _secondImageSource;

        public IsAvailableUpdateToCheckBoxIconConverter(string firstImageSource, string secondImageSource)
        {
            _firstImageSource = firstImageSource;
            _secondImageSource = secondImageSource;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imageSource = string.Empty;

            var isAvailable = (bool)value;

            if (isAvailable)
                imageSource = _firstImageSource;
            else
                imageSource = _secondImageSource;

            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
