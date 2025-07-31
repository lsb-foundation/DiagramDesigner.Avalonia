using Avalonia;
using Avalonia.VisualTree;

namespace DiagramDesigner;

public class VisualTreeHelper
{
    public static TVisual GetParent<TVisual>(Visual visual) where TVisual : Visual
    {
        if (visual == null) return null;
        var parent = visual.GetVisualParent();
        if (parent is TVisual find) return find;
        return GetParent<TVisual>(parent);
    }
}
