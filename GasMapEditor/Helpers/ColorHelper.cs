using Avalonia.Media;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GasMapEditor.Helpers;

internal static class ColorHelper
{
    public static Color Blend(this Color color1, Color color2)
    {
        var a = Math.Max(color1.A, color2.A);
        var r = (byte)(0xff - ((0xff - color1.R) * (0xff - color2.R) / 0xff));
        var g = (byte)(0xff - ((0xff - color1.G) * (0xff - color2.G) / 0xff));
        var b = (byte)(0xff - ((0xff - color1.B) * (0xff - color2.B) / 0xff));
        return new Color(a, r, g, b);
    }
}

internal class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Color.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
