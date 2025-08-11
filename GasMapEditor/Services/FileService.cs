using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace GasMapEditor.Services;

internal class FileService(Window mainWindow) : IFileService
{
    private readonly Window _mainWindow = mainWindow;

    public async Task<IStorageFile> OpenFileAsync(FilePickerOpenOptions option)
    {
        var files = await _mainWindow.StorageProvider.OpenFilePickerAsync(option);
        return files.Count >= 1 ? files[0] : null;
    }

    public async Task<IStorageFile> SaveFileAsync(FilePickerSaveOptions option)
    {
        return await _mainWindow.StorageProvider.SaveFilePickerAsync(option);
    }
}
