using DiagramDesigner;
using GasMapEditor.Services;

namespace GasMapEditor.ViewModels;

internal class PreviewWindowViewModel : ViewModelBase
{
    private readonly GasMapDriver _driver;

    public PreviewWindowViewModel(GasMapDriver driver)
    {
        _driver = driver;
    }

    public IDiagramViewModel DiagramViewModel { get; set; }

    public void Initialize()
    {
        if (DiagramViewModel == null) return;
        _driver.Diagram = DiagramViewModel;
        _driver.Initialize();
    }
}
