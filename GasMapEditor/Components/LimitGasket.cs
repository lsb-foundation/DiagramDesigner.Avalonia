using DiagramDesigner;
using System.ComponentModel;

namespace GasMapEditor.Components;

[Description("限流垫片")]
internal class LimitGasket : Communicating
{
    public LimitGasket()
    {
        Type = ComponentType.LimitGasket;
        ItemHeight = 30;
        ItemWidth = 30;
    }

    public override void Init()
    {
        var top = new FullyCreatedConnectorInfo(this, ConnectorOrientation.Top)
        {
            Type = ConnectorType.Both
        };
        var bottom = new FullyCreatedConnectorInfo(this, ConnectorOrientation.Bottom)
        {
            Type = ConnectorType.Both
        };
        Connectors.Add(top);
        Connectors.Add(bottom);
    }
}
