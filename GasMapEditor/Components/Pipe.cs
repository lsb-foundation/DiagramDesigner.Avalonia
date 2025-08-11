using Avalonia.Media;
using DiagramDesigner;
using GasMapEditor.Models;
using GasMapEditor.Services;
using System.Collections.Generic;

namespace GasMapEditor.Components;

internal class Pipe(ConnectionViewModel connection) : IGasMapComponent
{
    public ConnectionViewModel Connection { get; } = connection;
    public HashSet<Gas> CurrentGases { get; set; } = [];
    public ComponentBase Node1 { get; } = connection.SourceConnectorInfo.DataItem as ComponentBase;
    public ComponentBase Node2 { get; } = connection.SinkConnectorInfoFully?.DataItem as ComponentBase;

    public List<ComponentBase> OutputNodes
    {
        get
        {
            var list = new List<ComponentBase>();
            if (IsOutput(Node1)) list.Add(Node1);
            if (IsOutput(Node2)) list.Add(Node2);
            return list;
        }
    }

    public List<ComponentBase> InputNodes
    {
        get
        {
            var list = new List<ComponentBase>();
            if (!IsOutput(Node1)) list.Add(Node1);
            if (!IsOutput(Node2)) list.Add(Node2);
            return list;
        }
    }

    public void AttachToComponent()
    {
        Attach(Node1);
        Attach(Node2);
    }

    private void Attach(ComponentBase node)
    {
        if (node is null) return;

        FullyCreatedConnectorInfo connector = node == Node1 ?
            Connection.SourceConnectorInfo :
            Connection.SinkConnectorInfoFully;

        if (node is Junction || node is Communicating) //连接的是节点/双向连接组件
        {
            if (!node.InputPipes.Contains(this)) node.InputPipes.Add(this);
            if (!node.OutputPipes.Contains(this)) node.OutputPipes.Add(this);
        }
        else if (connector == node.Output && !node.OutputPipes.Contains(this))   //连接的是出口
        {
            node.OutputPipes.Add(this);
        }
        else if (connector == node.Input && !node.InputPipes.Contains(this))
        {
            node.InputPipes.Add(this);
        }
    }

    private bool IsOutput(ComponentBase node)
    {
        if (node is null) return false;
        FullyCreatedConnectorInfo connector = node == Node1 ?
            Connection.SourceConnectorInfo :
            Connection.SinkConnectorInfoFully;
        return connector == node.Output;
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

    public void Update()
    {
        Color color = GasMapService.MixGasColor(CurrentGases, GasMapService.ConnectionDefaultColor);
        Connection.LineColor = color;
    }

    public void Vacuumize()
    {
        Connection.LineColor = GasMapService.VacuumColor;
    }

    public bool TryGetConnectedInput(ComponentBase source, out ComponentBase connected)
    {
        connected = Node1 == source ? Node2 : Node1;
        if (connected is null) return false;
        if (connected is Junction || connected is Communicating) return true; //节点和联通组件不区分Input/Output
        if (IsOutput(connected)) return false;    //连接到出口
        return true;
    }

    public bool TryGetConnectedOutput(ComponentBase source, out ComponentBase connected)
    {
        connected = Node1 == source ? Node2 : Node1;
        if (connected is null) return false;
        if (connected is Junction || connected is Communicating) return true;  //节点和联通组件不区分Input/Output
        if (!IsOutput(connected)) return false;    //没有连接到出口
        return true;
    }
}
