using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace DiagramDesigner;

[PseudoClasses(":prepared")]
internal class DesignerItemPanel : Panel
{
    public void PreparedToConnect()
    {
        PseudoClasses.Set(":prepared", true);
    }

    public void ReleaseConnect()
    {
        PseudoClasses.Set(":prepared", false);
    }
}
