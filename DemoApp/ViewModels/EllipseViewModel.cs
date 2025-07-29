
using Avalonia.Media;
using DiagramDesigner;

namespace DemoApp.ViewModels;

internal class EllipseViewModel : DesignerItemViewModelBase
{
    private Color fillColor = Colors.Green;
    public Color FillColor
    {
        get => fillColor; 
        set => SetProperty(ref fillColor, value);
    }
}
