
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using DiagramDesigner;

namespace DemoApp.ViewModels;

internal partial class EllipseViewModel : DesignerItemViewModelBase
{
    [ObservableProperty]
    private Color fillColor = Colors.Green;
}
