using Avalonia;
using Avalonia.Input;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DiagramDesigner;

// Wraps info of the dragged object into a class
public class DragObject : DataObject
{
    public Size? DesiredSize { get; set; }

    public Size? DesireMinSize { get; set; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type ContentType { get; set; }

    public string Icon { get; set; }

    public string Text { get; set; }
}
