using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesigner;

internal class DesignerCanvas : Canvas
{
    private ConnectionViewModel partialConnection;
    private List<Connector> connectorsHit = [];
    private Connector sourceConnector;
    private Point? rubberbandSelectionStartPoint = null;

    private DesignerItemPanel designerItemPanelHit;
    private DesignerItemPanel connectorPanelHit;

    public DesignerCanvas()
    {
        DragDrop.SetAllowDrop(this, true);
        DragDrop.DropEvent.AddClassHandler<DesignerCanvas>(OnDrop);
    }

    public Connector SourceConnector
    {
        get { return sourceConnector; }
        set
        {
            if (sourceConnector != value)
            {
                sourceConnector = value;
                connectorsHit.Add(sourceConnector);
                FullyCreatedConnectorInfo sourceDataItem = sourceConnector.DataContext as FullyCreatedConnectorInfo;

                var point = PointHelper.GetPointForConnector(sourceDataItem);
                partialConnection = new ConnectionViewModel(sourceDataItem, new PartCreatedConnectionInfo(point));
                sourceDataItem.DataItem.Parent.AddItemCommand.Execute(partialConnection);
            }
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.Pointer.IsPrimary)
        {
            //if we are source of event, we are rubberband selecting
            if (e.Source == this)
            {
                // in case that this click is the start for a 
                // drag operation we cache the start point
                rubberbandSelectionStartPoint = e.GetPosition(this);

                if (e.KeyModifiers != KeyModifiers.Control)
                {
                    IDiagramViewModel vm = DataContext as IDiagramViewModel;
                    vm.ClearSelectedItemsCommand.Execute(null);
                }
                e.Handled = true;
            }
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        WeakReferenceMessenger.Default.Send<DoneDrawingMessage>();

        if (rubberbandSelectionStartPoint != null)
        {
            rubberbandSelectionStartPoint = null;
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            if (adornerLayer != null)
            {
                adornerLayer.Children.Clear();
            }
        }

        if (sourceConnector != null)
        {
            FullyCreatedConnectorInfo sourceDataItem = sourceConnector.DataContext as FullyCreatedConnectorInfo;
            if (connectorsHit.Count() == 2)
            {
                Connector sinkConnector = connectorsHit.Last();
                FullyCreatedConnectorInfo sinkDataItem = sinkConnector.DataContext as FullyCreatedConnectorInfo;

                int indexOfLastTempConnection = sinkDataItem.DataItem.Parent.Items.Count - 1;
                sinkDataItem.DataItem.Parent.RemoveItemCommand.Execute(
                    sinkDataItem.DataItem.Parent.Items[indexOfLastTempConnection]);
                var connectionViewModel = new ConnectionViewModel(sourceDataItem, sinkDataItem) { ZIndex = -1 };
                sinkDataItem.DataItem.Parent.AddItemCommand.Execute(connectionViewModel);
            }
            else
            {
                //Need to remove last item as we did not finish drawing the path
                int indexOfLastTempConnection = sourceDataItem.DataItem.Parent.Items.Count - 1;
                sourceDataItem.DataItem.Parent.RemoveItemCommand.Execute(
                    sourceDataItem.DataItem.Parent.Items[indexOfLastTempConnection]);
            }
        }
        connectorsHit = new List<Connector>();
        sourceConnector = null;

        connectorPanelHit?.ReleaseConnect();
        designerItemPanelHit?.ReleaseConnect();
        connectorPanelHit = null;
        designerItemPanelHit = null;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (SourceConnector != null)
        {
            var point = e.GetCurrentPoint(this);
            if (point.Properties.IsLeftButtonPressed)
            {
                Point currentPoint = e.GetPosition(this);
                partialConnection.SinkConnectorInfo = new PartCreatedConnectionInfo(currentPoint);
                HitTesting(currentPoint);
            }
        }
        else
        {
            // point value is set we do have one
            if (this.rubberbandSelectionStartPoint.HasValue)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    adornerLayer.Children.Clear();
                    // create rubberband adorner
                    var endPoint = e.GetPosition(this);
                    var adorner = new RubberbandAdorner(this, rubberbandSelectionStartPoint.Value, endPoint);
                    adornerLayer.Children.Add(adorner);
                    AdornerLayer.SetAdornedElement(adorner, this);
                }
            }
        }
        e.Handled = true;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        double width = 0, height = 0;

        foreach (Control element in Children)
        {
            double left = Canvas.GetLeft(element);
            double top = Canvas.GetTop(element);
            left = double.IsNaN(left) ? 0 : left;
            top = double.IsNaN(top) ? 0 : top;

            //measure desired size for each child
            element.Measure(constraint);

            Size desiredSize = element.DesiredSize;
            if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
            {
                width = Math.Max(width, left + desiredSize.Width);
                height = Math.Max(height, top + desiredSize.Height);
            }
        }
        // add margin
        width += 10;
        height += 10;
        return new Size(width, height);
    }

    private void HitTesting(Point hitPoint)
    {
        var hitObject = this.InputHitTest(hitPoint) as Visual;
        while (hitObject != null && hitObject is not DesignerCanvas)
        {
            if (hitObject is Connector connector)
            {
                if (!connectorsHit.Contains(connector))
                    connectorsHit.Add(connector);
            }
            if (hitObject is DesignerItemPanel panel)
            {
                panel.PreparedToConnect();
                if (panel.DataContext is DesignerItemViewModelBase)
                {
                    if (designerItemPanelHit != panel)
                    {
                        designerItemPanelHit?.ReleaseConnect();
                        designerItemPanelHit = panel;
                    }
                }
                else if (panel.DataContext is FullyCreatedConnectorInfo)
                {
                    if (connectorPanelHit != panel)
                    {
                        connectorPanelHit?.ReleaseConnect();
                        connectorPanelHit = panel;
                    }
                }
            }
            hitObject = hitObject.GetVisualParent();
        }
    }

    private void OnDrop(DesignerCanvas canvas, DragEventArgs e)
    {
        DragObject dragObject = e.Data as DragObject;
        if (dragObject != null)
        {
            (DataContext as IDiagramViewModel).ClearSelectedItemsCommand.Execute(null);
            Point position = e.GetPosition(this);
            DesignerItemViewModelBase itemBase = (DesignerItemViewModelBase)Activator.CreateInstance(dragObject.ContentType);
            itemBase.Left = Math.Max(0, position.X - itemBase.ItemWidth / 2);
            itemBase.Top = Math.Max(0, position.Y - itemBase.ItemHeight / 2);
            itemBase.IsSelected = true;
            if (dragObject.DesiredSize != null)
            {
                itemBase.ItemWidth = dragObject.DesiredSize.Value.Width;
                itemBase.ItemHeight = dragObject.DesiredSize.Value.Height;
            }
            (DataContext as IDiagramViewModel).AddItemCommand.Execute(itemBase);
        }
        e.Handled = true;
    }
}
