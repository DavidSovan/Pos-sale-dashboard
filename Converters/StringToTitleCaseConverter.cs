
using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace PosSale.Converters;
public class StringToTitleCaseConverter : IValueConverter
{
    public static StringToTitleCaseConverter Instance { get; } = new StringToTitleCaseConverter();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrEmpty(str))
        {
            return culture.TextInfo.ToTitleCase(str.ToLower());
        }
        return value;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}