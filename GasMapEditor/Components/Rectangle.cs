using Avalonia.Media;
using DiagramDesigner;
using GasMapEditor.Services;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GasMapEditor.Components;

[Description("矩形框")]
internal class Rectangle : DesignerItemViewModelBase, IGasMapElement
{
    private Color fillColor = GasMapService.ComponentDefaultColor;

    [Editable(true)]
    [DisplayName("填充色")]
    public Color FillColor
    {
        get => fillColor;
        set => SetProperty(ref fillColor, value);
    }

    public override void Init() { }
}
