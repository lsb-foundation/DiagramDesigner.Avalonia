using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace DiagramDesigner;

public partial class DiagramControl : UserControl
{
    public DiagramControl()
    {
        InitializeComponent();
    }
}

public class DiagramControlTemplateSelector : IDataTemplate
{
    public IDataTemplate DesignerItemTemplate { get; set; }

    public IDataTemplate ConnectionTemplate { get; set; }

    public Control Build(object param)
    {
        return param switch
        {
            DesignerItemViewModelBase => DesignerItemTemplate.Build(param),
            ConnectionViewModel => ConnectionTemplate.Build(param),
            _ => new TextBlock { Text = "Unknown item type" }
        };
    }

    public bool Match(object data) => data is SelectableDesignerItemViewModelBase;
}