using System;
using System.Windows.Data;
using SS.Surface.Classes;

namespace SS.Surface.Converters
{
    public class StringToQRCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = value as string;
            return QRCode.CreateQRCode(val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
