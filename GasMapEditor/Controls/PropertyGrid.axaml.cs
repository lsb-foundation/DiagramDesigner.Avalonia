using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GasMapEditor;

/// <summary>
/// 简单的属性编辑面板
/// </summary>
public partial class PropertyGrid : UserControl
{
    public PropertyGrid()
    {
        InitializeComponent();
    }

    static PropertyGrid()
    {
        ItemProperty.Changed.AddClassHandler<PropertyGrid>(ItemPropertyChanged);
    }

    public static readonly StyledProperty<object> ItemProperty =
        AvaloniaProperty.Register<PropertyGrid, object>(nameof(Item), default);

    public object Item
    {
        get => this.GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    internal ObservableCollection<PropertyViewModel> Properties { get; } = [];

    static void ItemPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        var propertyGrid = sender as PropertyGrid;
        propertyGrid.Properties.Clear();

        if (e.NewValue != null)
        {
            foreach (var property in e.NewValue.GetType().GetProperties())
            {
                if (property.GetCustomAttribute<EditableAttribute>() is null) continue;
                propertyGrid.Properties.Add(new PropertyViewModel(e.NewValue, property));
            }
        }
    }
}

internal partial class PropertyViewModel : ObservableObject
{
    private readonly object _item;
    private readonly PropertyInfo _property;

    public PropertyViewModel(object item, PropertyInfo property)
    {
        _item = item;
        _property = property;

        var attr = property.GetCustomAttribute<DisplayNameAttribute>();
        DisplayName = attr != null ? attr.DisplayName : property.Name;

        GetPropertyType();
    }

    public Type BasicType { get; private set; }

    public PropertyType Type { get; private set; }

    public string DisplayName { get; }

    public object Value
    {
        get => _property.GetValue(_item);
        set => SetValue(value);
    }

    void SetValue(object value)
    {
        try
        {
            if (value == null) return;

            string valueType = value.GetType().Name;
            var convertedValue = (valueType, BasicType.Name) switch
            {
                ("Single" or "Double" or "Decimal", "Int32") => Convert.ToInt32(value),
                ("Double" or "Decimal", "Single") => Convert.ToSingle(value),
                ("Decimal", "Double") => Convert.ToDouble(value),
                _ => value
            };
            _property.SetValue(_item, convertedValue);
            OnPropertyChanged();
        }
        catch
        {
            throw new Exception("SetValue error");
        }
    }

    void GetPropertyType()
    {
        BasicType = _property.PropertyType;
        if (_property.PropertyType.IsGenericType)
        {
            if (_property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var types = _property.PropertyType.GetGenericArguments();
                if (types.Length != 1) return;
                BasicType = types[0];
            }
        }

        Type = BasicType.Name switch
        {
            "Int32" or "Single" or "Double" => PropertyType.Number,
            "String" => PropertyType.Text,
            "Boolean" => PropertyType.CheckBox,
            "Color" => PropertyType.Color,
            _ => PropertyType.Unknown
        };
    }
}

internal enum PropertyType
{
    Unknown,
    Number,
    CheckBox,
    Text,
    Color
}

internal class ProptyGridTemplateSelector : IDataTemplate
{
    [Content]
    public Dictionary<string, DataTemplate> Templates { get; } = [];

    public Control Build(object param)
    {
        if (param is PropertyViewModel viewModel)
        {
            var key = viewModel.Type.ToString();
            if (Templates.TryGetValue(key, out DataTemplate template))
            {
                return template.Build(param);
            }
        }
        
        return new TextBlock { Text = "Unsupported param" };
    }

    public bool Match(object data) =>
        data is PropertyViewModel;
}