using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using DiagramDesigner.Helpers;

namespace DiagramDesigner;

public class DragAndDropProps : AvaloniaObject 
{
    #region EnabledForDrag

    static DragAndDropProps()
    {
        EnableForDragProperty.Changed.AddClassHandler<Control>(OnEnabledForDragChanged);
    }

    public static readonly AttachedProperty<bool> EnableForDragProperty =
        AvaloniaProperty.RegisterAttached<DragAndDropProps, StyledElement, bool>("EnableForDrag", false);

    public static void SetEnableForDrag(StyledElement element, bool value) =>
        element.SetValue(EnableForDragProperty, value);

    public static bool GetEnableForDrag(StyledElement element) =>
        element.GetValue(EnableForDragProperty);

    private static void OnEnabledForDragChanged(Control d, AvaloniaPropertyChangedEventArgs e)
    {
        if((bool)e.NewValue)
        {
            d.PointerPressed += OnPointerPressed;
            d.PointerMoved += OnPointerMoved;
        }
        else
        {
            d.PointerPressed -= OnPointerPressed;
            d.PointerMoved -= OnPointerMoved;
        }
    }
    #endregion

    #region DragStartPoint

    public static readonly AttachedProperty<Point?> DragStartPointProperty =
        AvaloniaProperty.RegisterAttached<DragAndDropProps, StyledElement, Point?>("DragStartPoint");

    public static void SetDragStartPoint(StyledElement element, Point? value) =>
        element.SetValue(DragStartPointProperty, value);

    public static Point? GetDragStartPoint(StyledElement element) =>
        element.GetValue(DragStartPointProperty);

    #endregion

    static void OnPointerMoved(object sender, PointerEventArgs e)
    {
        var control = sender as Control;
        Point? dragStartPoint = GetDragStartPoint(control);

        var point = e.GetCurrentPoint(control);
        if (!point.Properties.IsLeftButtonPressed)
        {
            dragStartPoint = null;
        }

        if (dragStartPoint.HasValue && control?.DataContext is ToolBoxData toolboxData)
        {
            DragObject dataObject = new DragObject();
            dataObject.ContentType = toolboxData.Type;
            dataObject.DesiredSize = toolboxData.DesiredSize;
            dataObject.DesireMinSize = toolboxData.DesiredMinSize;
            dataObject.Icon = toolboxData.Icon;
            dataObject.Text = toolboxData.Text;
            DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Copy);
            e.Handled = true;
        }
    }

    static void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        var control = sender as Control;
        SetDragStartPoint(control, e.GetPosition(control));
    }
}
