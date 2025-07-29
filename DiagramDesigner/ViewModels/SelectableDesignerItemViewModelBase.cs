using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesigner;

public interface ISelectItems
{
    RelayCommand<object> SelectItemCommand { get;  }
}

public abstract class SelectableDesignerItemViewModelBase : ViewModelBase, ISelectItems
{
    private bool isSelected;

    public SelectableDesignerItemViewModelBase(int id, IDiagramViewModel parent)
    {
        this.Id = id;
        this.Parent = parent;
        Init();
    }

    public SelectableDesignerItemViewModelBase()
    {
        Init();
    }

    public List<SelectableDesignerItemViewModelBase> SelectedItems
    {
        get { return Parent.SelectedItems; }
    }

    public IDiagramViewModel Parent { get; set; }
    public RelayCommand<object> SelectItemCommand { get; private set; }
    public int Id { get; set; }

    public bool IsSelected
    {
        get
        {
            return isSelected;
        }
        set
        {
            if (isSelected != value)
            {
                isSelected = value;
                OnPropertyChanged();
                if (isSelected)
                {
                    SelectedTime = DateTime.Now;
                }
            }
        }
    }

    public DateTime SelectedTime { get; private set; }

    public abstract double Left { get; set; }

    public abstract double Top { get; set; }

    private int zIndex;
    public int ZIndex
    {
        get => zIndex;
        set => SetProperty(ref zIndex, value);
    }

    private void ExecuteSelectItemCommand(object param)
    {
        SelectItem((bool)param, !IsSelected);
    }
    
    private void SelectItem(bool newselect, bool select)
    {
        if (newselect)
        {
            foreach (var designerItemViewModelBase in Parent.SelectedItems.ToList())
            {
                designerItemViewModelBase.isSelected = false;
            }
        }

        IsSelected = select;
    }

    private void Init()
    {
        SelectItemCommand = new RelayCommand<object>(ExecuteSelectItemCommand);
    }
}
