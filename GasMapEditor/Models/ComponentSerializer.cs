using Avalonia.Media;
using GasMapEditor.Components;
using GasMapEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GasMapEditor.Models;

internal class ComponentSerializer
{
    public ComponentSerializer() { }

    public ComponentSerializer(ComponentBase component)
    {
        DesignerItemType = component.GetType().FullName;
        Left = component.Left;
        Top = component.Top;
        Angle = component.Angle;
        ItemWidth = component.ItemWidth;
        ItemHeight = component.ItemHeight;
        FillColor = component.FillColor;

        GetConnectors(component);
        GetCustomProperties(component);
    }

    public string DesignerItemType { get; set; }
    public double Left { get; set; }
    public double Top { get; set; }
    public double Angle { get; set; }
    public double ItemWidth { get; set; }
    public double ItemHeight { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color FillColor { get; set; }

    public List<ConnectorSerializer> Connectors { get; set; } = [];
    public Dictionary<string, object> CustomProperties { get; set; } = [];

    private void GetConnectors(ComponentBase component)
    {
        foreach (var connector in component.Connectors)
        {
            Connectors.Add(new ConnectorSerializer(connector));
        }
    }

    private void GetCustomProperties(ComponentBase component)
    {
        foreach (var property in component.GetType().GetProperties())
        {
            var customPropertyAttr = property.GetCustomAttribute<CustomPropertyAttribute>();
            if (customPropertyAttr != null)
            {
                var propertyValue = property.GetValue(component);
                if (propertyValue != null)
                {
                    CustomProperties.Add(property.Name, propertyValue);
                }
            }
        }
    }

    public ComponentBase CreateComponentInstance()
    {
        Type type = Type.GetType(DesignerItemType) ?? throw new TypeUnloadedException();
        ComponentBase component = Activator.CreateInstance(type) as ComponentBase ?? throw new NullReferenceException();

        component.Left = Left;
        component.Top = Top;
        component.ItemWidth = ItemWidth;
        component.ItemHeight = ItemHeight;
        component.Angle = Angle;
        component.FillColor = FillColor;

        foreach (var customProperty in CustomProperties)
        {
            var property = type.GetProperty(customProperty.Key);
            var jsonElement = (JsonElement)customProperty.Value;
            var propertyValue = JsonSerializer.Deserialize(jsonElement, property.PropertyType);
            property.SetValue(component, propertyValue);
        }

        return component;
    }
}
