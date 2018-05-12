using System;
using System.Globalization;
using System.Security;
using System.Windows.Data;

namespace NullVoidCreations.WpfHelpers.Converters
{
    public class SecureStringToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var secureString = value as SecureString;
            return secureString == null ? string.Empty : secureString.ToUnsecureString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var unsecureString = value as string;
            if (string.IsNullOrEmpty(unsecureString))
                unsecureString = string.Empty;

            return unsecureString.ToSecureString();
        }
    }
}
