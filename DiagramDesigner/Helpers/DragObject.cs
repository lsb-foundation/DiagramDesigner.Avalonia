using Avalonia;
using Avalonia.Input;
using System;

namespace DiagramDesigner;

// Wraps info of the dragged object into a class
public class DragObject : DataObject
{
    public Size? DesiredSize { get; set; }

    public Size? DesireMinSize { get; set; }

    public Type ContentType { get; set; }

    public string Icon { get; set; }

    public string Text { get; set; }
}
