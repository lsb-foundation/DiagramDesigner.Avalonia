using DiagramDesigner;
using GasMapEditor.Helpers;
using System.ComponentModel;

namespace GasMapEditor.Components;

[Description("文本框")]
internal class TextLabel : DesignerItemViewModelBase, IGasMapElement
{
    public TextLabel()
    {
        ItemWidth = 80;
        ItemHeight = 30;
    }

    private string text;

    [CustomProperty]
    public string Text
    {
        get => text;
        set => SetProperty(ref text, value);
    }

    public override void Init() { }
}
