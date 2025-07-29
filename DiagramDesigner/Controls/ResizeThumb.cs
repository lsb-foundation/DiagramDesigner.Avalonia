using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;

namespace DiagramDesigner;

internal class ResizeThumb : Thumb
{
    protected override void OnDragDelta(VectorEventArgs e)
    {
        base.OnDragDelta(e);

        var designerItem = DataContext as DesignerItemViewModelBase;
        if (designerItem != null)
        {
            double xScale, yScale;
            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    xScale = (designerItem.ItemWidth - e.Vector.X) / designerItem.ItemWidth;
                    if (designerItem.ItemWidth * xScale > designerItem.MinItemWidth)
                    {
                        designerItem.ItemWidth *= xScale;
                        designerItem.Left += e.Vector.X;
                    }
                    break;
                case HorizontalAlignment.Right:
                    xScale = (designerItem.ItemWidth + e.Vector.X) / designerItem.ItemWidth;
                    if (designerItem.ItemWidth * xScale > designerItem.MinItemWidth)
                    {
                        designerItem.ItemWidth *= xScale;
                    }
                    break;
                default:
                    break;
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    yScale = (designerItem.ItemHeight - e.Vector.Y) / designerItem.ItemHeight;
                    if (designerItem.ItemHeight * yScale > designerItem.MinItemHeight)
                    {
                        designerItem.ItemHeight *= yScale;
                        designerItem.Top += e.Vector.Y;
                    }
                    break;
                case VerticalAlignment.Bottom:
                    yScale = (designerItem.ItemHeight + e.Vector.Y) / designerItem.ItemHeight;
                    if (designerItem.ItemHeight * yScale > designerItem.MinItemHeight)
                    {
                        designerItem.ItemHeight *= yScale;
                    }
                    break;
                default:
                    break;
            }

            e.Handled = true;
        }
    }
}