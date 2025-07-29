using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiagramDesigner;

public partial class ConnectorContainer : UserControl
{
    private Canvas rootCanvas;

    public ConnectorContainer()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<ObservableCollection<FullyCreatedConnectorInfo>> ConnectorsProperty =
        AvaloniaProperty.Register<ConnectorContainer, ObservableCollection<FullyCreatedConnectorInfo>>(nameof(Connectors), default);

    public ObservableCollection<FullyCreatedConnectorInfo> Connectors
    {
        get => this.GetValue(ConnectorsProperty);
        set => SetValue(ConnectorsProperty, value);
    }

    public static readonly StyledProperty<bool> IsReadyProperty =
        AvaloniaProperty.Register<ConnectorContainer, bool>(nameof(IsReady), false);

    public bool IsReady
    {
        get => this.GetValue(IsReadyProperty);
        set => SetValue(IsReadyProperty, value);
    }

    private void RootCanvas_Loaded(object sender, RoutedEventArgs e)
    {
        rootCanvas = sender as Canvas;
        SetConnectorPosition();
        SizeChanged += ConnectorContainer_SizeChanged;
    }

    private void SetConnectorPosition()
    {
        foreach (var connector in rootCanvas.Children.OfType<ContentPresenter>())
        {
            if (connector.DataContext is FullyCreatedConnectorInfo vm)
            {
                switch (vm.GetOrientation())
                {
                    case ConnectorOrientation.Left:
                        Canvas.SetLeft(connector, 0);
                        Canvas.SetTop(connector, vm.DataItem.ItemHeight * vm.GetYRatio() - ConnectorInfoBase.ConnectorHeight / 2);
                        break;
                    case ConnectorOrientation.Top:
                        Canvas.SetLeft(connector, vm.DataItem.ItemWidth * vm.GetXRatio() - ConnectorInfoBase.ConnectorWidth / 2);
                        Canvas.SetTop(connector, 0);
                        break;
                    case ConnectorOrientation.Right:
                        Canvas.SetLeft(connector, vm.DataItem.ItemWidth - ConnectorInfoBase.ConnectorWidth);
                        Canvas.SetTop(connector, vm.DataItem.ItemHeight * vm.GetYRatio() - ConnectorInfoBase.ConnectorHeight / 2);
                        break;
                    case ConnectorOrientation.Bottom:
                        Canvas.SetLeft(connector, vm.DataItem.ItemWidth * vm.GetXRatio() - ConnectorInfoBase.ConnectorWidth / 2);
                        Canvas.SetTop(connector, vm.DataItem.ItemHeight - ConnectorInfoBase.ConnectorHeight);
                        break;
                }
            }
        }
        IsReady = true;
    }

    private void ConnectorContainer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetConnectorPosition();
    }
}