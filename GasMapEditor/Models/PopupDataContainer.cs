using CommunityToolkit.Mvvm.ComponentModel;
using GasMapEditor.ViewModels;

namespace GasMapEditor.Models;

internal partial class PopupDataContainer : ViewModelBase
{
    [ObservableProperty]
    private double width;

    [ObservableProperty]
    private double height;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private object data;
}
