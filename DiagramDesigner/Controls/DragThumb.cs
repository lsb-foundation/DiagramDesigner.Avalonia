using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesigner;

internal class DragThumb : Thumb
{
    public List<SelectableDesignerItemViewModelBase> designerItems;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        SelectionProps.OnPointerPressed(this, e);
    }

    protected override void OnDragStarted(VectorEventArgs e)
    {
        base.OnDragStarted(e);

        var designerItem = DataContext as DesignerItemViewModelBase;
        if (designerItem != null && designerItem.IsSelected)
        {
            // we only move DesignerItems
            designerItems = designerItem.Parent.SelectedItems.ToList();
        }
        e.Handled = true;
    }

    protected override void OnDragDelta(VectorEventArgs e)
    {
        base.OnDragDelta(e);

        if (designerItems != null)
        {
            double minLeft = double.MaxValue;
            double minTop = double.MaxValue;

            foreach (DesignerItemViewModelBase item in designerItems.OfType<DesignerItemViewModelBase>())
            {
                double left = item.Left;
                double top = item.Top;
                minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);

                double deltaHorizontal = Math.Max(-minLeft, e.Vector.X);
                double deltaVertical = Math.Max(-minTop, e.Vector.Y);
                item.Left += deltaHorizontal;
                item.Top += deltaVertical;

                // prevent dragging items out of groupitem
                if (item.Parent is IDiagramViewModel && item.Parent is DesignerItemViewModelBase)
                {
                    DesignerItemViewModelBase groupItem = (DesignerItemViewModelBase)item.Parent;
                    if (item.Left + item.ItemWidth >= groupItem.ItemWidth) item.Left = groupItem.ItemWidth - item.ItemWidth;
                    if (item.Top + item.ItemHeight >= groupItem.ItemHeight) item.Top = groupItem.ItemHeight - item.ItemHeight;
                }
            }
            e.Handled = true;
        }
    }
}
