using Avalonia.Data.Converters;
using GasMapEditor.Components;
using System;
using System.Globalization;

namespace GasMapEditor.Converters;

internal class ValveStateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (ValveState)value == ValveState.Open;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? ValveState.Open : ValveState.Closed;
    }
}
