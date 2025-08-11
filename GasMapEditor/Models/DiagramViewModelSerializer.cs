using DiagramDesigner;
using GasMapEditor.Components;
using GasMapEditor.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GasMapEditor.Models;

internal class DiagramViewModelSerializer
{
    private int _id = 0;

    public DiagramViewModelSerializer() { }

    public DiagramViewModelSerializer(IDiagramViewModel diagramViewModel)
    {
        Width = diagramViewModel.Option.Width;
        Height = diagramViewModel.Option.Height;

        SetComponents(diagramViewModel);
        SetConnections(diagramViewModel);
        SetInterlocks(diagramViewModel);
    }

    public double Width { get; set; }
    public double Height { get; set; }
    public List<ComponentSerializer> Components { get; set; } = [];
    public List<ConnectionViewModelSerializer> Connections { get; set; } = [];
    public List<InterlockSerializer> Interlocks { get; set; } = [];

    private void SetComponents(IDiagramViewModel diagramViewModel)
    {
        foreach (var component in diagramViewModel.Items.OfType<ComponentBase>())
        {
            var componentSerializer = new ComponentSerializer(component);
            foreach (var connectorSerializer in componentSerializer.Connectors)
            {
                var connector = connectorSerializer.GetConnector();
                connectorSerializer.Id = ++_id;
                connector.SerializerIdentity = _id;
            }
            Components.Add(componentSerializer);
        }
    }

    private void SetConnections(IDiagramViewModel diagramViewModel)
    {
        foreach (var connection in diagramViewModel.Items.OfType<ConnectionViewModel>())
        {
            if (connection.SinkConnectorInfo is FullyCreatedConnectorInfo sinkConnector &&
                connection.SourceConnectorInfo.SerializerIdentity != 0 &&
                sinkConnector.SerializerIdentity != 0)
            {
                var connectionSerializer = new ConnectionViewModelSerializer()
                {
                    SourceConnectorId = connection.SourceConnectorInfo.SerializerIdentity,
                    SinkConnectorId = sinkConnector.SerializerIdentity,
                    LineWidth = connection.LineWidth
                };
                Connections.Add(connectionSerializer);
            }
        }
    }

    private void SetInterlocks(IDiagramViewModel diagramViewModel)
    {
        foreach (var interlock in diagramViewModel.Items.OfType<IInterlock>())
        {
            if (interlock.Interlocks.Count > 0)
            {
                Interlocks.Add(new InterlockSerializer(interlock));
            }
        }
    }

    public IDiagramViewModel CreateDiagramViewModelInstance()
    {
        var diagramViewModel = new DiagramViewModel();
        diagramViewModel.Option.Width = Width;
        diagramViewModel.Option.Height = Height;

        InitializeComponents(diagramViewModel);
        InitializeConnections(diagramViewModel);
        InitializeInterlocks(diagramViewModel);

        return diagramViewModel;
    }

    private void InitializeComponents(IDiagramViewModel diagramViewModel)
    {
        foreach (var componentSerializer in Components)
        {
            var component = componentSerializer.CreateComponentInstance();
            foreach (var connector in component.Connectors)
            {
                var serializer = componentSerializer.Connectors.FirstOrDefault(c =>
                    c.Type == connector.Type &&
                    c.Orientation == connector.Orientation &&
                    c.XRatio == connector.XRatio &&
                    c.YRatio == connector.YRatio);

                if (serializer != null)
                {
                    connector.SerializerIdentity = serializer.Id;
                }
            }

            component.Parent = diagramViewModel;
            diagramViewModel.Items.Add(component);
        }
    }

    private void InitializeConnections(IDiagramViewModel diagramViewModel)
    {
        IEnumerable<FullyCreatedConnectorInfo> connectors =
            diagramViewModel.Items.OfType<ComponentBase>()
            .SelectMany(c => c.Connectors);

        foreach (var connectionSerializer in Connections)
        {
            var sourceConnector = connectors.FirstOrDefault(c => c.SerializerIdentity == connectionSerializer.SourceConnectorId);
            var sinkConnector = connectors.FirstOrDefault(c => c.SerializerIdentity == connectionSerializer.SinkConnectorId);

            if (sourceConnector != null && sinkConnector != null)
            {
                var connectionViewModel = new ConnectionViewModel(sourceConnector, sinkConnector)
                {
                    LineWidth = connectionSerializer.LineWidth,
                    Parent = diagramViewModel
                };
                diagramViewModel.Items.Add(connectionViewModel);
            }
        }
    }

    private void InitializeInterlocks(IDiagramViewModel diagramViewModel)
    {
        foreach (var interlockSerializer in Interlocks)
        {
            if (GetInterlock(diagramViewModel, interlockSerializer.InterlockId) is IInterlock interlock)
            {
                foreach (int id in interlockSerializer.Interlocks)
                {
                    if (GetInterlock(diagramViewModel, id) is IInterlock t)
                    {
                        interlock.Interlocks.Add(t);
                    }
                }
            }
        }
    }

    private static IInterlock GetInterlock(IDiagramViewModel diagramViewModel, int interlockId) =>
        diagramViewModel.Items.OfType<IInterlock>()
                .FirstOrDefault(i => i.InterlockId == interlockId);

    public static async Task<IDiagramViewModel> GetDiagramFromFileAsync(string file)
    {
        using var stream = File.OpenRead(file);
        var diagramObjects = await JsonSerializer.DeserializeAsync<DiagramViewModelSerializer>(stream);
        return diagramObjects.CreateDiagramViewModelInstance();
    }

    public static async Task SaveDiagramToFileAsync(IDiagramViewModel diagram, string filePath)
    {
        var diagramSerializer = new DiagramViewModelSerializer(diagram);
        using var stream = File.OpenWrite(filePath);
        await JsonSerializer.SerializeAsync(stream, diagramSerializer);
    }
}
