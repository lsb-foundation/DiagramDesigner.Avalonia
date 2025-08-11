using DiagramDesigner;
using System.Collections.Generic;
using System.ComponentModel;

namespace GasMapEditor.Components;

[Description("气泵")]
internal class Pump : Valve
{
    public Pump() : base()
    {
        ItemHeight = 80;
        ItemWidth = 80;
        Type = ComponentType.Pump;
    }

    public ValveState MBPState { get; set; }

    public ValveState DPState { get; set; }

    public override ValveState State
    {
        get
        {
            return MBPState == ValveState.Open && DPState == ValveState.Open ?
                ValveState.Open : ValveState.Closed;
        }
        set
        {
            if (value == State) return;
            if (value == ValveState.Open)
            {
                MBPState = ValveState.Open;
                DPState = ValveState.Open;
            }
            else
            {
                MBPState = ValveState.Closed;
                DPState = ValveState.Closed;
            }

            OnPropertyChanged(nameof(MBPState));
            OnPropertyChanged(nameof(DPState));
            OnPropertyChanged(nameof(State));

            OnStateChanged(this);
        }
    }

    public override void Init()
    {
        var top = new FullyCreatedConnectorInfo(this, ConnectorOrientation.Top)
        {
            Type = ConnectorType.Input
        };
        var bottom = new FullyCreatedConnectorInfo(this, ConnectorOrientation.Bottom)
        {
            Type = ConnectorType.Output
        };
        Connectors.Add(top);
        Connectors.Add(bottom);
        Input = top;
        Output = bottom;
    }

    public override void Vacuumize()
    {
        var queue = new Queue<ComponentBase>();
        var visited = new HashSet<ComponentBase>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            var component = queue.Dequeue();

            if (visited.Contains(component)) continue;
            visited.Add(component);

            foreach (var pipe in component.InputPipes)
            {
                pipe.Vacuumize();

                if (!pipe.TryGetConnectedOutput(component, out ComponentBase connected)) continue;
                if (connected is Furnace) continue;
                if (connected is Valve v && v.State == ValveState.Closed) continue;

                connected.Vacuumize();
                queue.Enqueue(connected);
            }
        }
    }

    public override void OnAddedToDiagram()
    {
        //No need generating index, nothing to do...
    }
}
