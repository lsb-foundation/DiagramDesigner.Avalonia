using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DiagramDesigner;
using DiagramDesigner.Helpers;
using GasMapEditor.Components;
using GasMapEditor.Helpers;
using GasMapEditor.Messages;
using GasMapEditor.Models;
using GasMapEditor.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GasMapEditor.ViewModels;

internal partial class MainWindowViewModel : ViewModelBase
{
    private readonly IFileService _fileService;
    private readonly PreviewWindowViewModel _previewWindowViewModel;

    private CancellationTokenSource _cancellationTokenSource;

    public MainWindowViewModel(IFileService fileService, PreviewWindowViewModel previewWindowViewModel)
    {
        Initialize();
        _fileService = fileService;
        _previewWindowViewModel = previewWindowViewModel;
    }

    [ObservableProperty]
    private IDiagramViewModel diagramViewModel;

    [ObservableProperty]
    private ObservableCollection<ToolBoxData> toolboxDatas = [];

    [ObservableProperty]
    private AppMessage message;

    public void Initialize()
    {
        DiagramViewModel = new DiagramViewModel();
        DiagramViewModel.Option.Width = 900;
        DiagramViewModel.Option.Height = 600;
        ArcOrthogonalPathFinder.DiagramViewModel = DiagramViewModel;
        ConnectionViewModel.PathFinder = new ArcOrthogonalPathFinder();
        var components = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterface("IGasMapElement") != null);
        foreach (var component in components)
        {
            var attr = component.GetCustomAttribute<DescriptionAttribute>();
            if (attr != null)
            {
                ToolboxDatas.Add(new ToolBoxData(attr.Description, "", component, new Avalonia.Size(0, 0)));
            }
        }
    }

    [RelayCommand]
    private void Rotate()
    {
        if (DiagramViewModel?.SelectedItem is not ComponentBase node) return;
        var angle = node.Angle;
        angle += 90;
        if (angle >= 360) angle = 0;
        node.Angle = angle;
    }

    [RelayCommand]
    private void AdjustLineWidth(bool? add = true)
    {
        if (DiagramViewModel?.SelectedItem is not ConnectionViewModel connection) return;
        if (add.HasValue && add.Value) connection.LineWidth++;
        else
        {
            if (connection.LineWidth == 1) return;
            else connection.LineWidth--;
        }
    }

    [RelayCommand]
    private void ViewInterlock()
    {
        if (DiagramViewModel?.SelectedItem is not IInterlock interlock) return;
        foreach (var item in interlock.Interlocks)
        {
            if (item is DesignerItemViewModelBase designerItem)
            {
                designerItem.IsSelected = true;
            }
        }
    }

    [RelayCommand]
    private void Delete()
    {
        if (DiagramViewModel == null) return;
        DiagramViewModel.RemoveSelectedItems();
    }

    [RelayCommand]
    private void SetInterlock()
    {
        if (DiagramViewModel?.SelectedItem is null) return;
        var interlocks = DiagramViewModel.SelectedItems.OfType<IInterlock>();
        foreach (var interlock in interlocks)
        {
            interlock.Interlocks.AddInterlocks(interlocks.Except([interlock]));
        }
    }

    [RelayCommand]
    private void ReleaseInterlock()
    {
        if (DiagramViewModel?.SelectedItem is null) return;
        var interlocks = DiagramViewModel.SelectedItems.OfType<IInterlock>();
        foreach (var interlock in interlocks)
        {
            foreach (var i in interlock.Interlocks)
            {
                i.Interlocks.Remove(interlock);
            }
            interlock.Interlocks.Clear();
        }
    }

    [RelayCommand]
    private void Preview()
    {
        if (DiagramViewModel is null) return;

        _previewWindowViewModel.DiagramViewModel = DiagramViewModel;
        WeakReferenceMessenger.Default.Send(new OpenPreviewWindow
        {
            ViewModel = _previewWindowViewModel
        });
    }

    [RelayCommand]
    private void ClearMap()
    {
        if (DiagramViewModel is null) return;
        DiagramViewModel.Items.Clear();
    }

    [RelayCommand]
    private async Task OpenFile()
    {
        FilePickerOpenOptions option = new()
        {
            AllowMultiple = false,
            FileTypeFilter = [FilePickerTypes.Json],
        };

        var storage = await _fileService.OpenFileAsync(option);
        if (storage != null)
        {
            try
            {
                var path = storage.TryGetLocalPath();
                var diagram = await DiagramViewModelSerializer.GetDiagramFromFileAsync(path);
                DiagramViewModel = diagram;
            }
            catch (IOException ex)
            {
                ShowMessage("File error: " + ex.Message, true);
            }
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (DiagramViewModel is null) return;
        if (DiagramViewModel.Items.Count == 0) return;

        FilePickerSaveOptions option = new()
        {
            SuggestedFileName = "气路图文件.json",
            DefaultExtension = ".json",
            FileTypeChoices = [FilePickerTypes.Json],
            ShowOverwritePrompt = true
        };

        var storage = await _fileService.SaveFileAsync(option);
        if (storage != null)
        {
            try
            {
                var path = storage.TryGetLocalPath();
                await DiagramViewModelSerializer.SaveDiagramToFileAsync(DiagramViewModel, path);
            }
            catch (IOException ex)
            {
                ShowMessage("File error:" + ex.Message, true);
            }
        }
    }

    private async void ShowMessage(string message, bool isError)
    {
        try
        {
            _cancellationTokenSource?.Cancel();

            var appMessage = new AppMessage
            {
                Message = message,
                IsError = isError
            };

            _cancellationTokenSource = new CancellationTokenSource();
            Message = appMessage;
            await Task.Delay(TimeSpan.FromSeconds(3), _cancellationTokenSource.Token);
            Message = null;
        }
        catch (TaskCanceledException) { }
    }
}
