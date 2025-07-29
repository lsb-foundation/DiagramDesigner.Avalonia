using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace DiagramDesigner;

internal class SelectionProps : AvaloniaObject
{
    #region EnabledForSelection

    static SelectionProps()
    {
        EnableForSelectionProperty.Changed.AddClassHandler<Control>(OnEnabledForDragChanged);
    }

    public static readonly AttachedProperty<bool> EnableForSelectionProperty =
        AvaloniaProperty.RegisterAttached<SelectionProps, StyledElement, bool>("EnableForSelection", false);

    public static void SetEnableForSelection(StyledElement element, bool value) =>
        element.SetValue(EnableForSelectionProperty, value);

    public static bool GetEnableForSelection(StyledElement element) =>
        element.GetValue(EnableForSelectionProperty);

    private static void OnEnabledForDragChanged(Control d, AvaloniaPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue)
        {
            d.PointerPressed += OnPointerPressed;
        }
        else
        {
            d.PointerPressed -= OnPointerPressed;
        }
    }

    #endregion

    public static void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if ((sender as Visual)?.DataContext is not SelectableDesignerItemViewModelBase selectableItem) return;

        if ((e.KeyModifiers & (KeyModifiers.Control)) != KeyModifiers.None)
        {
            selectableItem.IsSelected = !selectableItem.IsSelected;
        }

        if (!selectableItem.IsSelected)
        {
            foreach (SelectableDesignerItemViewModelBase item in selectableItem.Parent.SelectedItems)
            {
                if (item is IDiagramViewModel dvm)
                {
                    foreach (SelectableDesignerItemViewModelBase gItem in dvm.Items)
                    {
                        gItem.IsSelected = false;
                    }
                }
                if (selectableItem.Parent is SelectableDesignerItemViewModelBase sdiVm)
                {
                    sdiVm.IsSelected = false;
                }
                item.IsSelected = false;
            }
            if (selectableItem is IDiagramViewModel diagramViewModel)
            {
                foreach (SelectableDesignerItemViewModelBase gItem in diagramViewModel.Items)
                {
                    gItem.IsSelected = false;
                }
            }
            selectableItem.Parent.SelectedItems.Clear();
            selectableItem.IsSelected = true;
        }
    }
}
