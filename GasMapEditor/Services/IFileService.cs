using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace GasMapEditor.Services;

internal interface IFileService
{
    Task<IStorageFile> OpenFileAsync(FilePickerOpenOptions option);
    Task<IStorageFile> SaveFileAsync(FilePickerSaveOptions option);
}
