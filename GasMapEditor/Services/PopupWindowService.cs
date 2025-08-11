using Avalonia.Controls;
using GasMapEditor.Models;
using System.Threading.Tasks;

namespace GasMapEditor.Services;

internal class PopupWindowService(Window mainWindow) : IPopupService
{
    private readonly Window _mainWindow = mainWindow;

    public async Task<bool> ShowDialog(PopupDataContainer dataContainer)
    {
        Window win = new PopupWindow
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ShowInTaskbar = false,
            DataContext = dataContainer
        };
        return await win.ShowDialog<bool>(_mainWindow);
    }
}
