using Avalonia;
using Avalonia.Controls;
using DiagramDesigner.Helpers;
using System.Collections.Generic;

namespace DemoApp;

public partial class ToolBox : UserControl
{
    public ToolBox()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<List<ToolBoxData>> ToolBoxDataCollectionProperty =
        AvaloniaProperty.Register<ToolBox, List<ToolBoxData>>(nameof(ToolBoxDataCollection), default);

    public List<ToolBoxData> ToolBoxDataCollection
    {
        get => this.GetValue(ToolBoxDataCollectionProperty);
        set => SetValue(ToolBoxDataCollectionProperty, value);
    }
}