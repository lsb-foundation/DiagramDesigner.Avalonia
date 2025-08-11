using Avalonia.Media;
using GasMapEditor.Helpers;
using GasMapEditor.Models;
using System.Collections.Generic;
using System.Linq;

namespace GasMapEditor.Services;

internal class GasMapService
{
    public static Color ComponentDefaultColor { get; } = Color.FromRgb(0x3f, 0x6c, 0xdf);
    public static Color ConnectionDefaultColor { get; } = Color.FromRgb(0x80, 0x80, 0x80);
    public static Color VacuumColor { get; } = Colors.Yellow;

    public static Color MixGasColor(HashSet<Gas> gasSet, Color defaultColor)
    {
        return gasSet.Count > 0 ?
            gasSet.Select(t => t.Color).Aggregate((c1, c2) => c1.Blend(c2)) :
            defaultColor;
    }
}
