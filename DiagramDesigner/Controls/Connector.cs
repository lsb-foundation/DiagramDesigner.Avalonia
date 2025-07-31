using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace DiagramDesigner;

internal class Connector : TemplatedControl
{
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        var canvas = VisualTreeHelper.GetParent<DesignerCanvas>(this);
        if (canvas != null)
        {
            canvas.SourceConnector = this;
        }

        e.Handled = true;
    }

    public ConnectorOrientation Orientation { get; set; }
}