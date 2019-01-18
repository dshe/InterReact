using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UwpRealtimeVolume.Utility
{
    public class DoubleToColorConverter : IValueConverter
    {
        private readonly SolidColorBrush white = new SolidColorBrush(Colors.White);
        private readonly SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
        private readonly SolidColorBrush red = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush green = new SolidColorBrush(Colors.Green);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var v = (double?) value;
            if (v == null)
                return gray; // unknown
            if (v > 0)
                return green; // increase
            if (v < 0)
                    return red; // decrease
            return white; // no change
        }

        // not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
