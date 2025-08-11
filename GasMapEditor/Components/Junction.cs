using Avalonia.Media;
using DiagramDesigner;
using GasMapEditor.Services;
using System.ComponentModel;

namespace GasMapEditor.Components;

[Description("连接点")]
internal class Junction : ComponentBase
{
    public Junction() : base(ComponentType.Junction)
    {
        ItemWidth = 10;
        ItemHeight = 10;
        FillColor = Colors.DarkSlateGray;
    }

    public override void Init()
    {
        Connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Top, ConnectorType.Both));
        Connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Bottom, ConnectorType.Both));
        Connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Left, ConnectorType.Both));
        Connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Right, ConnectorType.Both));
    }

    public override void Update()
    {
        var color = GasMapService.MixGasColor(CurrentGases, Colors.DarkSlateGray);
        FillColor = color;
    }
}
