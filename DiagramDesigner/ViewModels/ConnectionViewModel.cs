using Avalonia;
using Avalonia.Media;
using DiagramDesigner.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DiagramDesigner;

public class ConnectionViewModel : SelectableDesignerItemViewModelBase
{
    private FullyCreatedConnectorInfo sourceConnectorInfo;
    private ConnectorInfoBase sinkConnectorInfo;
    private Point sourceB;
    private Point sourceA;
    private List<Point> connectionPoints;
    private Rect area;

    public ConnectionViewModel(int id, IDiagramViewModel parent,
        FullyCreatedConnectorInfo sourceConnectorInfo, FullyCreatedConnectorInfo sinkConnectorInfo) : base(id, parent)
    {
        Init(sourceConnectorInfo, sinkConnectorInfo);
    }

    public ConnectionViewModel(FullyCreatedConnectorInfo sourceConnectorInfo, ConnectorInfoBase sinkConnectorInfo)
    {
        Init(sourceConnectorInfo, sinkConnectorInfo);
    }

    public static IPathFinder PathFinder { get; set; }

    public bool IsFullConnection => sinkConnectorInfo is FullyCreatedConnectorInfo;

    public Point SourceA
    {
        get => sourceA;
        set
        {
            if (sourceA != value)
            {
                sourceA = value;
                UpdateArea();
                OnPropertyChanged();
            }
        }
    }

    public Point SourceB
    {
        get => sourceB;
        set
        {
            if (sourceB != value)
            {
                sourceB = value;
                UpdateArea();
                OnPropertyChanged();
            }
        }
    }

    public List<Point> ConnectionPoints
    {
        get => connectionPoints;
        private set => SetProperty(ref connectionPoints, value);
    }

    public Rect Area
    {
        get => area;
        private set
        {
            if (area != value)
            {
                area = value;
                UpdateConnectionPoints();
                OnPropertyChanged();
                OnPropertyChanged(nameof(Left));
                OnPropertyChanged(nameof(Top));
            }
        }
    }

    public override double Left 
    { 
        get => Area.Left; 
        set { } 
    }

    public override double Top 
    { 
        get => Area.Top;
        set { }
    }

    private Color lineColor = Colors.Gray;
    public Color LineColor
    {
        get => lineColor;
        set => SetProperty(ref lineColor, value);
    }

    private double lineWidth = 2.0;
    public double LineWidth
    {
        get => lineWidth;
        set => SetProperty(ref lineWidth, value);
    }

    public ConnectorInfo ConnectorInfo(ConnectorOrientation orientation, double left, double top, Point position)
    {
        return new ConnectorInfo()
        {
            Orientation = orientation,
            DesignerItemSize = new Size(sourceConnectorInfo.DataItem.ItemWidth, sourceConnectorInfo.DataItem.ItemHeight),
            DesignerItemLeft = left,
            DesignerItemTop = top,
            Position = position
        };
    }

    public FullyCreatedConnectorInfo SourceConnectorInfo
    {
        get => sourceConnectorInfo;
        set
        {
            if (sourceConnectorInfo != value)
            {
                sourceConnectorInfo = value;
                SourceA = PointHelper.GetPointForConnector(this.SourceConnectorInfo);
                OnPropertyChanged();
                (sourceConnectorInfo.DataItem as INotifyPropertyChanged).PropertyChanged += new WeakEventHandler(ConnectorViewModel_PropertyChanged).Handler;
            }
        }
    }

    public ConnectorInfoBase SinkConnectorInfo
    {
        get => sinkConnectorInfo;
        set
        {
            if (sinkConnectorInfo != value)
            {
                sinkConnectorInfo = value;
                if (SinkConnectorInfo is FullyCreatedConnectorInfo)
                {
                    SourceB = PointHelper.GetPointForConnector((FullyCreatedConnectorInfo)SinkConnectorInfo);
                    (((FullyCreatedConnectorInfo)sinkConnectorInfo).DataItem as INotifyPropertyChanged).PropertyChanged += new WeakEventHandler(ConnectorViewModel_PropertyChanged).Handler;
                }
                else
                {
                    SourceB = ((PartCreatedConnectionInfo)SinkConnectorInfo).CurrentLocation;
                }
                OnPropertyChanged();
            }
        }
    }

    public FullyCreatedConnectorInfo SinkConnectorInfoFully => SinkConnectorInfo as FullyCreatedConnectorInfo;

    private void UpdateArea()
    {
        Area = new Rect(SourceA, SourceB);
    }

    private void UpdateConnectionPoints()
    {
        ConnectorInfo sourceInfo = ConnectorInfo(
              SourceConnectorInfo.GetOrientation(),
              SourceA.X,
              SourceA.Y,
              SourceA);

        if (IsFullConnection)
        {
            ConnectorInfo sinkInfo = ConnectorInfo(
                ((FullyCreatedConnectorInfo)SinkConnectorInfo).GetOrientation(),
                SourceB.X,
                SourceB.Y,
                SourceB);

            var pointList = PathFinder.GetConnectionLine(sourceInfo, sinkInfo, true);
            ConnectionPoints = pointList.Select(p => new Point(p.X - Area.Left, p.Y - Area.Top)).ToList();
        }
        else
        {
            var pointList = PathFinder.GetConnectionLine(sourceInfo, SourceB, ConnectorOrientation.Left);
            ConnectionPoints = pointList.Select(p => new Point(p.X - Area.Left, p.Y - Area.Top)).ToList();
        }
    }

    private void ConnectorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "ItemHeight":
            case "ItemWidth":
            case "Left":
            case "Top":
                SourceA = PointHelper.GetPointForConnector(this.SourceConnectorInfo);
                if (this.SinkConnectorInfo is FullyCreatedConnectorInfo)
                {
                    SourceB = PointHelper.GetPointForConnector((FullyCreatedConnectorInfo)this.SinkConnectorInfo);
                }
                break;
        }
    }

    private void Init(FullyCreatedConnectorInfo sourceConnectorInfo, ConnectorInfoBase sinkConnectorInfo)
    {
        //PathFinder ??= new OrthogonalPathFinder();
        PathFinder ??= new SimplePathFinder();
        this.Parent = sourceConnectorInfo.DataItem.Parent;
        this.SourceConnectorInfo = sourceConnectorInfo;
        this.SinkConnectorInfo = sinkConnectorInfo;
    }
}
