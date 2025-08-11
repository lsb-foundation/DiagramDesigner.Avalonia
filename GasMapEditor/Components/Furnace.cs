using DiagramDesigner;
using System.Collections.Generic;
using System.ComponentModel;

namespace GasMapEditor.Components;

[Description("炉体")]
internal class Furnace : ComponentBase
{
    public Furnace() : base(ComponentType.Furnace)
    {
        ItemHeight = 180;
        ItemWidth = 80;
    }

    //炉体有多路气体汇入
    public List<FullyCreatedConnectorInfo> Inputs { get; } = [];

    public override void Init()
    {
        for (double i = 0.55; i < 1; i += 0.05)
        {
            var connector = new FullyCreatedConnectorInfo(this, ConnectorOrientation.Left)
            {
                XRatio = 0,
                YRatio = i,
                Type = ConnectorType.Input
            };
            Connectors.Add(connector);
            Inputs.Add(connector);
        }
        var outPutConnector = new FullyCreatedConnectorInfo(this, ConnectorOrientation.Right)
        {
            XRatio = 1,
            YRatio = 0.75,
            Type = ConnectorType.Output
        };
        Connectors.Add(outPutConnector);
        Output = outPutConnector;
    }
}
