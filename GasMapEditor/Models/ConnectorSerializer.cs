using DiagramDesigner;

namespace GasMapEditor.Models;

internal class ConnectorSerializer
{
    private FullyCreatedConnectorInfo connector;

    public ConnectorSerializer() { }

    public ConnectorSerializer(FullyCreatedConnectorInfo connector)
    {
        this.connector = connector;
        Type = connector.Type;
        Orientation = connector.Orientation;
        XRatio = connector.XRatio;
        YRatio = connector.YRatio;
    }

    public int Id { get; set; }
    public ConnectorType Type { get; set; }
    public ConnectorOrientation Orientation { get; set; }
    public double XRatio { get; set; }
    public double YRatio { get; set; }

    public FullyCreatedConnectorInfo GetConnector() => connector;
}
