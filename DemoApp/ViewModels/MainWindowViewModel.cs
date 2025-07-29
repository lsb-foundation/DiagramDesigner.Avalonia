using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using DiagramDesigner;
using DiagramDesigner.Helpers;
using System.Collections.Generic;

namespace DemoApp.ViewModels;

internal partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        Initialize();
    }

    [ObservableProperty]
    private IDiagramViewModel diagramViewModel;

    [ObservableProperty]
    private List<ToolBoxData> toolBoxDataList;

    private void Initialize()
    {
        DiagramViewModel = new DiagramViewModel();
        var persist = new PersistDesignerItemViewModel
        {
            Left = 50,
            Top = 50
        };
       
        var settings = new SettingsDesignerItemViewModel
        {
            Left = 200,
            Top = 200
        };

        var connection = new ConnectionViewModel(persist.BottomConnector, settings.LeftConnector);

        DiagramViewModel.Add(persist);
        DiagramViewModel.Add(settings);
        DiagramViewModel.Add(connection);

        var toolBoxDatas = new List<ToolBoxData>
        {
            new ToolBoxData("persist", "", typeof(PersistDesignerItemViewModel), new Size(80, 80), description: "Drag me to add"),
            new ToolBoxData("settings", "", typeof(SettingsDesignerItemViewModel), new Size(80, 80), description: "Drag me to add"),
            new ToolBoxData("ellipse", "", typeof(EllipseViewModel), new Size(80, 80), description: "Drag me to add")
        };
        ToolBoxDataList = toolBoxDatas;
    }
}
