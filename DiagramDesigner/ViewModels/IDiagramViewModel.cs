using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiagramDesigner
{
    public interface IDiagramViewModel
    {
        RelayCommand<object> AddItemCommand { get; }
        RelayCommand<object> RemoveItemCommand { get;  }
        RelayCommand<object> ClearSelectedItemsCommand { get;  }
        List<SelectableDesignerItemViewModelBase> SelectedItems { get; }
        ObservableCollection<SelectableDesignerItemViewModelBase> Items { get; }
        SelectableDesignerItemViewModelBase SelectedItem { get; }

        void Add(SelectableDesignerItemViewModelBase item);
        void Remove(SelectableDesignerItemViewModelBase item);
    }
}
