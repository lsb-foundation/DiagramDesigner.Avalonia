using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace DiagramDesigner;

internal class RubberbandAdorner : Control
{
    private Point? startPoint;
    private Point? endPoint;
    private Pen rubberbandPen;

    private DesignerCanvas designerCanvas;

    public RubberbandAdorner(DesignerCanvas designerCanvas, Point? dragStartPoint, Point? dragEndPoint)
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

        if (this.startPoint.HasValue && this.endPoint.HasValue)
            context.DrawRectangle(Brushes.Transparent, rubberbandPen, new Rect(this.startPoint.Value, this.endPoint.Value));

        UpdateSelection();
    }

    private static TVisual GetParent<TVisual>(Visual visual) where TVisual : Visual
    {
        var parent = visual.GetVisualParent();
        if (parent is TVisual find) return find;
        return GetParent<TVisual>(parent);
    }

    private void UpdateSelection()
    {
        IDiagramViewModel vm = (designerCanvas.DataContext as IDiagramViewModel);
        Rect rubberBand = new Rect(startPoint.Value, endPoint.Value);
        ItemsControl itemsControl = GetParent<ItemsControl>(designerCanvas);

        foreach (SelectableDesignerItemViewModelBase item in vm.Items)
        {
            if (item is SelectableDesignerItemViewModelBase)
            {
                Visual container = itemsControl.ContainerFromItem(item);

                Rect itemBounds = container.GetTransformedBounds().Value.Bounds;
                Rect itemRect = new Rect(item.Left, item.Top, itemBounds.Width, itemBounds.Height);

                if (rubberBand.Contains(itemRect))
                {
                    Dispatcher.UIThread.Post(() => item.IsSelected = true);
                }
                else
                {
                    //if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                    //{
                    //    item.IsSelected = false;
                    //}
                }
            }
        }
    }
}
