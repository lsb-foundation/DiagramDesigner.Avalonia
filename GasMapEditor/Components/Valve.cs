using GasMapEditor.Helpers;
using GasMapEditor.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace GasMapEditor.Components;

[Description("气动阀")]
internal class Valve : ComponentBase, IInterlock
{
    public Valve() : base(ComponentType.Valve)
    {
        ItemWidth = 35;
        ItemHeight = 35;
    }

    private ValveState _state;
    public virtual ValveState State
    {
        get => _state;
        set
        {
            if (_state == value) return;
            if (value == ValveState.Open && Interlocks.IsLocked) return;
            IsLocked = value == ValveState.Open;
            SetProperty(ref _state, value);
            OnStateChanged(this);
        }
    }

    private int _index;

    [CustomProperty]
    [Editable(true)]
    [DisplayName("序号")]
    public int Index
    {
        get => _index;
        set => SetProperty(ref _index, value);
    }

    [CustomProperty]
    public int InterlockId { get; set; }

    public bool IsLocked { get; set; }

    public Interlocks Interlocks { get; } = [];

    public event Action<Valve> StateChanged;

    public override void Init()
    {
        AddBottomInput();
        AddTopOutput();
    }

    public void Open()
    {
        if (State == ValveState.Open) return;
        State = ValveState.Open;
    }

    public void Close()
    {
        if (State == ValveState.Closed) return;
        State = ValveState.Closed;
    }

    public void Toggle()
    {
        if (State == ValveState.Open) Close();
        else Open();
    }

    public virtual void OnStateChanged(Valve v)
    {
        StateChanged?.Invoke(v);
    }

    public override void OnAddedToDiagram()
    {
        Index = Parent.Items.OfType<Valve>().Max(t => t.Index) + 1;
    }
}