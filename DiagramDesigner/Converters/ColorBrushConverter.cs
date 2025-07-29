using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace DiagramDesigner;

public class ColorBrushConverter : IValueConverter
{
    static ColorBrushConverter()
    {
        Instance = new ColorBrushConverter();
    }

    public static ColorBrushConverter Instance { get; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Color color = (Color)value;
        return new SolidColorBrush(color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
