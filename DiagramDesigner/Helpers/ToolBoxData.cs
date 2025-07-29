using Avalonia;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DiagramDesigner.Helpers;

public class ToolBoxData
{
    public string Text { get; protected set; }

    public string Icon { get; protected set; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type Type { get; protected set; }

    public Size Size { get; protected set; }

    public Size? DesiredSize { get; protected set; }

    public Size? DesiredMinSize { get; protected set; }

    public string Description { get; protected set; }

    public ToolBoxData(string text, string icon,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]Type type,
        Size size, Size? desiredSize = null,
        Size? desiredMinSize = null,
        string description = null)
    {
        Text = text;
        Icon = icon;
        Type = type;
        Size = size;
        DesiredSize = desiredSize;
        DesiredMinSize = desiredMinSize;
        Description = description;
    }
}
