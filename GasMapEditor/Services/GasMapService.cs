using Avalonia.Media;
using DiagramDesigner;
using GasMapEditor.Components;
using GasMapEditor.Helpers;
using GasMapEditor.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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

    //public static IDiagramViewModel GetDiagramFromFile(string file)
    //{
    //    XmlSerializer serializer = new(typeof(DiagramDocument));
    //    FileInfo fileInfo = new(file);

    //    using TextReader reader = fileInfo.OpenText();
    //    var document = (DiagramDocument)serializer.Deserialize(reader);

    //    if (document is null) return null;
    //    var diagram = new BlockDiagramViewModel(document.DiagramItem, "xml");
    //    diagram.DiagramOption.LayoutOption.ShowGrid = false;

    //    document.Interlocks.TryParseInterlock(diagram);

    //    var interlockMax = diagram.Items.OfType<IInterlock>().Any() ?
    //        diagram.Items.OfType<IInterlock>().Max(i => i.InterlockId) : 0;

    //    Interlocks.SetMaxId(interlockMax);

    //    return diagram;
    //}


    //public static IEnumerable<Gas> LoadGasesFromFile(string file)
    //{
    //    XmlSerializer serializer = new(typeof(List<Gas>));
    //    using var stream = File.OpenRead(file);
    //    var obj = serializer.Deserialize(stream);
    //    if (obj is List<Gas> list)
    //    {
    //        return list;
    //    }
    //    return [];
    //}
}
