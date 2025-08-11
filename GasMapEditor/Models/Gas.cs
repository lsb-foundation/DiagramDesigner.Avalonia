using Avalonia.Media;
using GasMapEditor.Helpers;
using System.Text.Json.Serialization;

namespace GasMapEditor.Models;

internal class Gas
{
    public string Name { get; set; }

    public int Code { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Color { get; set; }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not Gas) return false;
        return Code == ((Gas)obj).Code;
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }
}
