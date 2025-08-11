using CommunityToolkit.Mvvm.ComponentModel;

namespace GasMapEditor.Models;

internal partial class AppMessage : ObservableObject
{
    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string message;
}
