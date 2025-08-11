using CommunityToolkit.Mvvm.ComponentModel;
using GasMapEditor.ViewModels;
using System.Collections.Generic;

namespace GasMapEditor.Models;

internal partial class MFCData : ViewModelBase
{
    [ObservableProperty]
    private float flow;

    [ObservableProperty]
    private string unit;

    public static List<string> FlowUnits => ["SLM", "SCCM"];
}
