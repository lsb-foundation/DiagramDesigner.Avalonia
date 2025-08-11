using DiagramDesigner;
using System.ComponentModel;

namespace GasMapEditor.Components;

[Description("连通组件")]
internal class Communicating : ComponentBase
{
    public Communicating() : base(ComponentType.Communicating)
    {
        ItemWidth = 50;
        ItemHeight = 50;
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
