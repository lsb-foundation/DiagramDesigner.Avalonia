namespace GasMapEditor.Components;

internal enum ComponentType
{
    //新增类型需要依次在后面添加
    Inlet,
    Valve,
    MFC,
    Furnace,
    Junction,
    Communicating,
    Pump,
    LimitGasket,
    HandValve,

    None = 9999
}
