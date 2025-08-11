using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiagramDesigner;
using GasMapEditor.Helpers;
using GasMapEditor.Models;
using GasMapEditor.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GasMapEditor.Components;

[Description("MFC")]
internal partial class MFC : ComponentBase
{
    public MFC() : base(ComponentType.MFC) { }

    private int index;

    [CustomProperty]
    [Editable(true)]
    [DisplayName("序号")]
    public int Index
    {
        get => index;
        set => SetProperty(ref index, value);
    }

    public IRelayCommand EditCommand { get; private set; }

    public override void Init()
    {
        EditCommand = new AsyncRelayCommand(Edit);
        AddTopOutput();
        AddBottomInput();
    }

    [ObservableProperty]
    private MFCData data = new();

    //[RelayCommand]
    private async Task Edit()
    {
        var data = new MFCData { Flow = Data.Flow };
        var container = new PopupDataContainer
        {
            Title = "MFC设定",
            Data = data,
            Width = 200,
            Height = 150
        };

        var popupService = (App.Current as App).Services.GetRequiredService<IPopupService>();
        if (await popupService.ShowDialog(container) == true)
        {
            Data.Flow = data.Flow;
            Data.Unit = data.Unit;
        }
    }

    public override void OnAddedToDiagram()
    {
        Index = Parent.Items.OfType<MFC>().Max(m => m.Index) + 1;
    }
}
