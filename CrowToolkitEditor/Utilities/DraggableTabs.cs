using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace CrowEngineUI.Utilities;

public class DraggableTabs : UserControl
{
    private bool _isClickingTab;
    private Point _lastPointerPosition;

    public void Draggable(object? sender, EventArgs e)
    {
        var draggableTabControl = this.FindControl<TabControl>("Draggable");

        foreach (TabItem? item in draggableTabControl?.Items!)
        {
            if (item == null) continue;

            item.PointerPressed += PointerPressed;
            item.PointerMoved += PointerMoved;
            item.PointerReleased += PointerReleased;
        }
    }

    private new void PointerPressed(object? sender, PointerPressedEventArgs pointerPressedEventArgs)
    {
        _isClickingTab = true;

        Console.WriteLine("Pressed!");
    }

    private new void PointerMoved(object? sender, PointerEventArgs pointerMoveEventArgs)
    {
        if (!_isClickingTab) return;

        Console.WriteLine("Moving!");
    }

    private new void PointerReleased(object? sender, PointerReleasedEventArgs pointerReleasedEventArgs)
    {
        _isClickingTab = false;

        Console.WriteLine("Released!");
    }
}