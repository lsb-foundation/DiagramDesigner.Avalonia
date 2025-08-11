
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using GasMapEditor.Messages;

namespace GasMapEditor.Views
{
    public partial class MainWindow : Window, IRecipient<OpenPreviewWindow>
    {
        public MainWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register(this);
        }

        async void IRecipient<OpenPreviewWindow>.Receive(OpenPreviewWindow message)
        {
            var preview = new PreviewWindow
            {
                DataContext = message.ViewModel,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false
            };
            message.ViewModel.Initialize();
            await preview.ShowDialog(this);
        }
    }
}