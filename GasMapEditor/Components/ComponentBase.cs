using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using DiagramDesigner;
using GasMapEditor.Helpers;
using GasMapEditor.Models;
using GasMapEditor.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GasMapEditor.Components;

internal partial class ComponentBase : DesignerItemViewModelBase, IGasMapComponent
{
    public ComponentBase(ComponentType kind) : base()
    {
        Type = kind;
        ItemWidth = 80;
        ItemHeight = 40;
        FillColor = GasMapService.ComponentDefaultColor;
    }

    public ComponentType Type { get; set; }

    public FullyCreatedConnectorInfo Input { get; set; }

    public FullyCreatedConnectorInfo Output { get; set; }

    public List<Pipe> OutputPipes { get; } = [];

    public List<Pipe> InputPipes { get; } = [];

    public HashSet<Gas> CurrentGases { get; set; } = [];

    private string _label;

    [CustomProperty]
    [Editable(true)]
    [DisplayName("标签")]
    public string Label
    {
        get => _label;
        set => SetProperty(ref _label, value);
    }

    private bool _isEnable = true;
    public bool IsEnable
    {
        get => _isEnable;
        set => SetProperty(ref _isEnable, value);
    }

    [ObservableProperty]
    private Color fillColor;

    public virtual void AddTopOutput()
    {
        Output = new(this, ConnectorOrientation.Top)
        {
            Type = ConnectorType.Output
        };
        Connectors.Add(Output);
    }

    public virtual void AddBottomInput()
    {
        Input = new(this, ConnectorOrientation.Bottom)
        {
            Type = ConnectorType.Input
        };
        Connectors.Add(Input);
    }

    public virtual void AddLeftInput()
    {
        Input = new(this, ConnectorOrientation.Left)
        {
            Type = ConnectorType.Input
        };
        Connectors.Add(Input);
    }

    public virtual void AddRightOutput()
    {
        Output = new(this, ConnectorOrientation.Right)
        {
            Type = ConnectorType.Output
        };
        Connectors.Add(Output);
    }

    public bool AddGas(IEnumerable<Gas> gasList)
    {
        bool containsAll = true;
        foreach (var gas in gasList)
        {
            if (CurrentGases.Add(gas))
            {
                containsAll = false;
            }
        }
        return containsAll;
    }

    public Pipe GetConnectedPipe(ComponentBase target)
    {
        foreach (var pipe in OutputPipes)
        {
            var connected = pipe.Node1 == this ? pipe.Node2 : pipe.Node1;
            if (connected == target)
            {
                return pipe;
            }
        }
        return null;
    }

    public virtual void Update()
    {
        var color = GasMapService.MixGasColor(CurrentGases, GasMapService.ComponentDefaultColor);
        FillColor = color;
    }

    public virtual void Vacuumize()
    {
        FillColor = GasMapService.VacuumColor;
    }

    /// <summary>
    /// BFS查找给定节点到开启的气泵的最短路径
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    public bool TryDetectPathToOpenPump(out List<ComponentBase> path)
    {
        path = null;
        if (this is Pump) return false;

        var predecessors = new Dictionary<ComponentBase, ComponentBase>();
        var visited = new HashSet<ComponentBase>();
        var queue = new Queue<ComponentBase>([this]);
        visited.Add(this);
        predecessors[this] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current is Pump p && p.State == ValveState.Open)
            {
                path = ReconstructPath(predecessors, current);
                return true;
            }

            foreach (var pipe in current.OutputPipes)
            {
                if (!pipe.TryGetConnectedInput(current, out ComponentBase connected)) continue;
                if (connected is Valve v && v.State == ValveState.Closed) continue;
                if (connected is Furnace) continue;
                if (!visited.Add(connected!)) continue;

                predecessors[connected!] = current;
                queue.Enqueue(connected!);
            }
        }
        return false;
    }

    private static List<ComponentBase> ReconstructPath(Dictionary<ComponentBase, ComponentBase> predecessors, ComponentBase endNode)
    {
        var path = new List<ComponentBase>();
        var current = endNode;

        while (current != null)
        {
            path.Add(current);
            current = predecessors[current];
        }

        // 反转路径，使其从起点到终点
        path.Reverse();
        return path;
    }
}
