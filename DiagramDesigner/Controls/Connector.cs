using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace DiagramDesigner;

internal class Connector : TemplatedControl
{
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        var canvas = GetDesignerCanvas(this);
        if (canvas != null)
        {
            canvas.SourceConnector = this;
        }

        e.Handled = true;
    }

    public ConnectorOrientation Orientation { get; set; }

    private DesignerCanvas GetDesignerCanvas(Visual element)
    {
        while (element != null && element is not DesignerCanvas)
            element = element.GetVisualParent();
        return element as DesignerCanvas;
    }
}