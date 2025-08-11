using CommunityToolkit.Mvvm.ComponentModel;

namespace DiagramDesigner;

public partial class DiagramOption : ObservableObject
{
    [ObservableProperty]
    private double width;

    [ObservableProperty]
    private double height;

    [ObservableProperty]
    private double zoomValue = 1.0;

    [ObservableProperty]
    private double zoomValueMax = 5.0;

    [ObservableProperty]
    private double zoomValueMin = 0.5;
}
