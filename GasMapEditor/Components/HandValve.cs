using System.ComponentModel;

namespace GasMapEditor.Components;

[Description("手阀")]
internal class HandValve : Valve
{
    public HandValve() : base()
    {
        //Type = ComponentType.HandValve;
        IsEnable = false;
        State = ValveState.Open;
    }

    public override void OnAddedToDiagram()
    {
        //No need generating index，nothing to do
    }
}
