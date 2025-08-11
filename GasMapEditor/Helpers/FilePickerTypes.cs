
using Avalonia.Platform.Storage;

namespace GasMapEditor.Helpers;

internal class FilePickerTypes
{
    public static FilePickerFileType Json = new("json")
    {
        Patterns = ["*.json"],
        AppleUniformTypeIdentifiers = ["public.json"],
        MimeTypes = ["application/json"]
    };
}
