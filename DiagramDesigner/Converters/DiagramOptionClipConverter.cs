using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace DiagramDesigner;

internal class DiagramOptionClipConverter : IValueConverter
{
    public static DiagramOptionClipConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DiagramOption option)
        {
            var rect = new Rect(0, 0, option.Width, option.Height);
            var geometry = new RectangleGeometry(rect);
            return geometry;
        }
        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
