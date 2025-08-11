using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace GasMapEditor;

public partial class PopupWindow : Window
{
    public PopupWindow()
    {
        InitializeComponent();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        this.Close(true);
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close(false);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        if (this.DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}