using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace GasMapEditor;

public class ImageIconButton : Button
{
    public static readonly StyledProperty<IImage> IconSourceProperty =
        AvaloniaProperty.Register<ImageIconButton, IImage>(nameof(IconSource), default);

    public IImage IconSource
    {
        get => this.GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<ImageIconButton, string>(nameof(Text), default);

    public string Text
    {
        get => this.GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}