namespace DiagramDesigner;

public class FullyCreatedConnectorInfo : ConnectorInfoBase
{
    private bool showConnectors = false;

    public FullyCreatedConnectorInfo(DesignerItemViewModelBase dataItem, ConnectorOrientation orientation)
        : base(orientation)
    {
        DataItem = dataItem;
    }

    public FullyCreatedConnectorInfo(DesignerItemViewModelBase dataItem, ConnectorOrientation orientation, ConnectorType type)
        : this(dataItem, orientation)
    {
        Type = type;
    }

    public FullyCreatedConnectorInfo(DesignerItemViewModelBase dataItem, double ratioX, double ratioY, ConnectorType type)
        : this(dataItem, ConnectorOrientation.None, type)
    {
        XRatio = ratioX;
        YRatio = ratioY;
    }

    public DesignerItemViewModelBase DataItem { get; private set; }

    public bool ShowConnectors
    {
        get => showConnectors;
        set => SetProperty(ref showConnectors, value);
    }

    private double xRatio;
    public double XRatio
    {
        get => xRatio;
        set => SetProperty(ref xRatio, value);
    }

    private double yRatio;
    public double YRatio
    {
        get => yRatio;
        set => SetProperty(ref yRatio, value);
    }

    public double GetXRatio()
    {
        if (XRatio != 0) return XRatio;
        return Orientation switch
        {
            ConnectorOrientation.Left => 0,
            ConnectorOrientation.Right => 1,
            ConnectorOrientation.Top => 0.5,
            ConnectorOrientation.Bottom => 0.5,
            _ => XRatio,
        };
    }

    public double GetYRatio()
    {
        if (YRatio != 0) return YRatio;
        return Orientation switch
        {
            ConnectorOrientation.Left => 0.5,
            ConnectorOrientation.Right => 0.5,
            ConnectorOrientation.Top => 0,
            ConnectorOrientation.Bottom => 1,
            _ => YRatio,
        };
    }

    public ConnectorOrientation GetOrientation()
    {
        if (Orientation != ConnectorOrientation.None) return Orientation;
        return (XRatio, YRatio) switch
        {
            (0, _) => ConnectorOrientation.Left,
            (1, _) => ConnectorOrientation.Right,
            (_, 0) => ConnectorOrientation.Top,
            (_, 1) => ConnectorOrientation.Bottom,
            _ => ConnectorOrientation.None
        };
    }
}
