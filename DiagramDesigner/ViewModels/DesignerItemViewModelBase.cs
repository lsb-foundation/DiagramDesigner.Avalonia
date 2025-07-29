using Avalonia.Media;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiagramDesigner;

public abstract class DesignerItemViewModelBase : SelectableDesignerItemViewModelBase
{
    private double left;
    private double top;
    private bool showConnectors = false;

    private double itemWidth = 65;
    private double itemHeight = 65;

    public DesignerItemViewModelBase(int id, IDiagramViewModel parent) : base(id, parent)
    {
        Init();
    }

    public DesignerItemViewModelBase(int id, IDiagramViewModel parent, double left, double top) : base(id, parent)
    {
        this.left = left;
        this.top = top;
        Init();
    }

    public DesignerItemViewModelBase(int id, IDiagramViewModel parent, double left, double top, double itemWidth, double itemHeight) : base(id, parent)
    {
        this.left = left;
        this.top = top;
        this.itemWidth = itemWidth;
        this.itemHeight = itemHeight;
        Init();
    }

    public DesignerItemViewModelBase(): base()
    {
        Init();
    }

    public ObservableCollection<FullyCreatedConnectorInfo> Connectors { get; } = [];

    public double ItemWidth
    {
        get => itemWidth;
        set => SetProperty(ref itemWidth, value);
    }

    public double ItemHeight
    {
        get => itemHeight;
        set => SetProperty(ref itemHeight, value);
    }

    private double angle;
    public double Angle
    {
        get => angle;
        set => SetProperty(ref angle, value);
    }

    public FullyCreatedConnectorInfo TopConnector =>
        Connectors.FirstOrDefault(c => c.Orientation == ConnectorOrientation.Top);

    public FullyCreatedConnectorInfo BottomConnector =>
        Connectors.FirstOrDefault(c => c.Orientation == ConnectorOrientation.Bottom);

    public FullyCreatedConnectorInfo LeftConnector =>
        Connectors.FirstOrDefault(c => c.Orientation == ConnectorOrientation.Left);

    public FullyCreatedConnectorInfo RightConnector =>
        Connectors.FirstOrDefault(c => c.Orientation == ConnectorOrientation.Right);

    public bool ShowConnectors
    {
        get => showConnectors;
        set
        {
            if (showConnectors != value)
            {
                showConnectors = value;
                foreach (var connector in Connectors)
                {
                    connector.ShowConnectors = value;
                }
                OnPropertyChanged();
            }
        }
    }

    public override double Left
    {
        get => left;
        set => SetProperty(ref left, value);
    }

    public override double Top
    {
        get => top;
        set => SetProperty(ref top, value);
    }

    private double minItemWidth;
    public double MinItemWidth
    {
        get => minItemWidth;
        set => SetProperty(ref minItemWidth, value);
    }

    private double minItemHeight;
    public double MinItemHeight
    {
        get => minItemHeight;
        set => SetProperty(ref minItemHeight, value);
    }
    
    public virtual void Init()
    {
        Connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Top));
        Connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Bottom));
        Connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Left));
        Connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Right));
    }
}
