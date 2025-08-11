using Avalonia.Media;
using GasMapEditor.Helpers;
using GasMapEditor.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GasMapEditor.Components;

[Description("气体入口")]
internal class GasInlet : ComponentBase
{
    public GasInlet() : base(ComponentType.Inlet) { }

    public string InletName => $"Inlet_({Gas?.Name ?? "TBD"})";

    [CustomProperty]
    public Gas Gas
    {
        get
        {
            if (CurrentGases.Count == 0) return null;
            return CurrentGases.ElementAt(0);
        }
        set
        {
            CurrentGases.Clear();
            CurrentGases.Add(value);
            OnPropertyChanged(nameof(InletName));
        }
    }

    [Editable(true)]
    [DisplayName("气体名称")]
    public string GasName
    {
        get => Gas?.Name;
        set
        {
            if (value is null) return;
            if (Gas is null) Gas = new Gas { Name = value };
            else Gas.Name = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(InletName));
        }
    }

    [Editable(true)]
    [DisplayName("气体编号")]
    public int GasCode
    {
        get => Gas?.Code ?? 0;
        set
        {
            if (Gas is null) Gas = new Gas { Code = value };
            else Gas.Code = value;
            OnPropertyChanged(nameof(GasCode));
        }
    }

    [Editable(true)]
    [DisplayName("气体颜色")]
    public Color? GasColor
    {
        get => Gas?.Color;
        set
        {
            if (value is null) return;
            if (Gas is null) Gas = new Gas { Color = value.Value };
            else Gas.Color = value.Value;
            FillColor = value.Value;
            OnPropertyChanged(nameof(GasColor));
        }
    }

    public override void Init()
    {
        AddTopOutput();
    }

    public override void Vacuumize()
    {
        //nothing todo
    }

    public override void OnAddedToDiagram()
    {
        GasCode = Parent.Items.OfType<GasInlet>().Max(t => t.GasCode) + 1;
    }
}
