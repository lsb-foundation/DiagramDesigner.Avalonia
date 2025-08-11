using DiagramDesigner;
using GasMapEditor.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace GasMapEditor.Services;

public class GasMapDriver
{
    private IDiagramViewModel _diagram;
    private readonly List<Pipe> _pipes = [];
    private readonly TimeSpan delay = TimeSpan.FromMilliseconds(50);
    private readonly Subject<Valve> subject = new Subject<Valve>();

    public GasMapDriver()
    {
        //实现事件防抖
        subject.Throttle(delay)
            .Subscribe(HandValveStateChange);
    }

    public IDiagramViewModel Diagram
    {
        get => _diagram;
        set
        {
            if (_diagram == value) return;

            foreach (var valve in _diagram?.Items.OfType<Valve>() ?? [])
            {
                valve.StateChanged -= OnValveStateChanged;
            }

            _diagram = value;
        }
    }

    public void Initialize()
    {
        if (_diagram is null) return;

        foreach (var node in _diagram.Items.OfType<ComponentBase>())
        {
            node.InputPipes.Clear();
            node.OutputPipes.Clear();

            if (node is Valve valve)
            {
                valve.StateChanged -= OnValveStateChanged;
                valve.StateChanged += OnValveStateChanged;
            }
        }

        _pipes.Clear();
        var connectors = _diagram.Items.OfType<ConnectionViewModel>();
        foreach (var connector in connectors)
        {
            var pipe = new Pipe(connector);
            pipe.AttachToComponent();
            _pipes.Add(pipe);
        }

        UpdateMapState();
        UpdateComponents();
    }

    private void OnValveStateChanged(Valve valve)
    {
        subject.OnNext(valve);
    }

    private void HandValveStateChange(Valve valve)
    {
        if (valve is not Pump)
        {
            var inputGasCount = valve.InputPipes.Count > 0 ? valve.InputPipes.Sum(p => p.CurrentGases.Count) : 0;
            if (valve.CurrentGases.Count == 0 &&
                inputGasCount == 0 &&
                !valve.TryDetectPathToOpenPump(out List<ComponentBase> path))
            {
                return;
            }
        }

        UpdateMapState();
        UpdateComponents();
    }

    private void UpdateComponents()
    {
        foreach (var pipe in _pipes)
        {
            pipe.Update();
        }

        foreach (var component in _diagram.Items.OfType<ComponentBase>())
        {
            component.Update();
        }

        foreach (var pump in _diagram.Items.OfType<Pump>().Where(p => p.State == ValveState.Open))
        {
            pump.Vacuumize();
        }
    }

    private void ClearMapGases()
    {
        // 重置所有组件的气体状态（除了入口）
        var components = _diagram.Items.OfType<IGasMapComponent>().Where(c => c is not GasInlet);
        foreach (var component in components)
        {
            component.CurrentGases.Clear();
        }
        //清空管道的气体状态
        foreach (var pipe in _pipes)
        {
            pipe.CurrentGases.Clear();
        }
    }

    private void UpdateMapState()
    {
        if (_diagram is null) return;

        ClearMapGases();

        foreach (var inlet in _diagram.Items.OfType<GasInlet>())
        {
            UpdateMapStateFrom(inlet);
        }
    }

    static void UpdateMapStateFrom(ComponentBase startNode)
    {
        Queue<ComponentBase> queue = new();
        queue.Enqueue(startNode);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current.CurrentGases.Count == 0) continue;

            foreach (var pipe in current.OutputPipes)
            {
                // 更新管道气体
                var pipeContainsAllGas = pipe.AddGas(current.CurrentGases);
                if (pipeContainsAllGas) continue;

                if (!pipe.TryGetConnectedInput(current, out ComponentBase connected)) continue;
                if (connected is Valve valve && valve.State == ValveState.Closed) continue;
                if (connected is Furnace)
                {
                    continue;
                }

                // 传播气体到下一个组件
                connected!.AddGas(current.CurrentGases);
                queue.Enqueue(connected);
            }
        }
    }
}
