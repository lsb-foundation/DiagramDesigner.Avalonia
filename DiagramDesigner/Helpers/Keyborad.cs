using Avalonia.Controls;
using Avalonia.Input;
using System.Collections.Generic;

namespace DiagramDesigner;

public static class Keyboard
{
    private static readonly HashSet<Key> keysDown = [];

    public static KeyModifiers Modifiers { get; private set; }

    public static bool IsKeyDown(Key key) => keysDown.Contains(key);

    public static void Register(Control control)
    {
        control.KeyDown += OnKeyDown;
        control.KeyUp += OnKeyUp;
    }

    public static void Unregister(Control control)
    {
        control.KeyDown -= OnKeyDown;
        control.KeyUp -= OnKeyUp;
    }

    static void OnKeyDown(object sender, KeyEventArgs e)
    {
        keysDown.Add(e.Key);
        if (e.KeyModifiers != KeyModifiers.None)
        {
            Modifiers = e.KeyModifiers;
        }
        e.Handled = true;
    }

    static void OnKeyUp(object sender, KeyEventArgs e)
    {
        keysDown.Remove(e.Key);
        e.Handled = true;
    }
}
