using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System;

namespace DiagramDesigner;

internal class RubberbandAdorner : Control
{
    private Point startPoint;
    private Point endPoint;
    private Pen rubberbandPen;

    private DesignerCanvas designerCanvas;

    public RubberbandAdorner(DesignerCanvas designerCanvas, Point dragStartPoint, Point dragEndPoint)
    {
        this.designerCanvas = designerCanvas;
        this.startPoint = dragStartPoint;
        this.endPoint = dragEndPoint;
        rubberbandPen = new Pen(Brushes.LightSlateGray, 1);
        rubberbandPen.DashStyle = new DashStyle([2], 1);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (startPoint != default && endPoint != default)
        {
            // Create a rectangle that's always positive in size (normalized)
            var rubberbandRect = new Rect(
                Math.Min(startPoint.X, endPoint.X),
                Math.Min(startPoint.Y, endPoint.Y),
                Math.Abs(endPoint.X - startPoint.X),
                Math.Abs(endPoint.Y - startPoint.Y));

            context.DrawRectangle(
                Brushes.Transparent,
                rubberbandPen,
                rubberbandRect);

            UpdateSelection(rubberbandRect);
        }
    }

    private void UpdateSelection(Rect rubberBand)
    {
        IDiagramViewModel vm = (designerCanvas.DataContext as IDiagramViewModel);
        ItemsControl itemsControl = VisualTreeHelper.GetParent<ItemsControl>(designerCanvas);

        foreach (SelectableDesignerItemViewModelBase item in vm.Items)
        {
            if (item is not null)
            {
                Visual container = itemsControl.ContainerFromItem(item);

                Rect itemBounds = container.GetTransformedBounds().Value.Bounds;
                Rect itemRect = new(item.Left, item.Top, itemBounds.Width, itemBounds.Height);

                if (rubberBand.Contains(itemRect))
                {
                    Dispatcher.UIThread.Post(() => item.IsSelected = true);
                }
            }
        }
    }
}
