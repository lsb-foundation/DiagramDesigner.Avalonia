using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace DiagramDesigner;

public class DiagramViewModel : ViewModelBase, IDiagramViewModel, IRecipient<DoneDrawingMessage>
{
    private readonly ObservableCollection<SelectableDesignerItemViewModelBase> items = [];

    public DiagramViewModel()
    {
        AddItemCommand = new RelayCommand<object>(ExecuteAddItemCommand);
        RemoveItemCommand = new RelayCommand<object>(ExecuteRemoveItemCommand);
        ClearSelectedItemsCommand = new RelayCommand<object>(ExecuteClearSelectedItemsCommand);
        CreateNewDiagramCommand = new RelayCommand<object>(ExecuteCreateNewDiagramCommand);

        items.CollectionChanged += Items_CollectionChanged;

        //Mediator.Instance.Register(this);
        WeakReferenceMessenger.Default.Register<DoneDrawingMessage>(this);
    }

    //[MediatorMessageSink("DoneDrawingMessage")]
    private void OnDoneDrawingMessage()
    {
        foreach (var item in Items.OfType<DesignerItemViewModelBase>())
        {
            item.ShowConnectors = false;
        }
    }

    void IRecipient<DoneDrawingMessage>.Receive(DoneDrawingMessage message) => OnDoneDrawingMessage();

    public RelayCommand<object> AddItemCommand { get; private set; }
    public RelayCommand<object> RemoveItemCommand { get; private set; }
    public RelayCommand<object> ClearSelectedItemsCommand { get; private set; }
    public RelayCommand<object> CreateNewDiagramCommand { get; private set; }

    public ObservableCollection<SelectableDesignerItemViewModelBase> Items => items;

    public List<SelectableDesignerItemViewModelBase> SelectedItems =>
        [.. Items.Where(x => x.IsSelected)];

    public SelectableDesignerItemViewModelBase SelectedItem =>
        SelectedItems.OrderByDescending(x => x.SelectedTime).FirstOrDefault();

    private void ExecuteAddItemCommand(object parameter)
    {
        if (parameter is SelectableDesignerItemViewModelBase item)
        {
            item.Parent = this;
            items.Add(item);
        }
    }

    private void ExecuteRemoveItemCommand(object parameter)
    {
        if (parameter is SelectableDesignerItemViewModelBase item)
        {
            items.Remove(item);
        }
    }

    private void ExecuteClearSelectedItemsCommand(object parameter)
    {
        foreach (SelectableDesignerItemViewModelBase item in Items)
        {
            item.IsSelected = false;
        }
    }

    private void ExecuteCreateNewDiagramCommand(object parameter)
    {
        Items.Clear();
    }

    public void Add(SelectableDesignerItemViewModelBase item)
    {
        if (item != null && !items.Contains(item))
        {
            item.Parent = this;
            items.Add(item);
        }
    }

    public void Remove(SelectableDesignerItemViewModelBase item)
    {
        if (item != null && items.Contains(item))
        {
            items.Remove(item);
        }
    }

    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems.OfType<SelectableDesignerItemViewModelBase>())
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems.OfType<SelectableDesignerItemViewModelBase>())
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsSelected")
        {
            OnPropertyChanged(nameof(SelectedItem));
        }
    }
}
