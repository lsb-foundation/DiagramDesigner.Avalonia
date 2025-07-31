
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using DiagramDesigner;

namespace DemoApp.ViewModels;

internal partial class EllipseViewModel : DesignerItemViewModelBase
{
    [ObservableProperty]
    private Color fillColor = Colors.Green;

    public override void Init()
    {
        Connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Left, ConnectorType.Input));
        Connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Right, ConnectorType.Output));
        Connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Top, ConnectorType.Both));
        Connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Bottom, ConnectorType.Both));
    }
}
