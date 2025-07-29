namespace DiagramDesigner;

public abstract class ConnectorInfoBase : ViewModelBase
{
    private static double connectorWidth = 16;
    private static double connectorHeight = 16;

    public ConnectorInfoBase(ConnectorOrientation orientation)
    {
        this.Orientation = orientation;
    }

    public ConnectorOrientation Orientation { get; private set; }

    public ConnectorType Type { get; set; }

    public static double ConnectorWidth
    {
        get { return connectorWidth; }
    }

    public static double ConnectorHeight
    {
        get { return connectorHeight; }
    }
}
