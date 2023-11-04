using System;
using Avalonia;
using Avalonia.Controls;

namespace CrowEngineProjectManager.Elements;

public partial class IconButton : Button
{
    
    protected override Type StyleKeyOverride => typeof(Button);
    
    public static readonly StyledProperty<Action> ButtonClickProperty =
        AvaloniaProperty.Register<IconButton, Action>(nameof(ButtonClick));
    
    public static readonly StyledProperty<string> IconKindProperty =
        AvaloniaProperty.Register<IconButton, string>(nameof(IconKind));
    
    public static readonly StyledProperty<string> ButtonTextProperty =
        AvaloniaProperty.Register<IconButton, string>(nameof(ButtonText));
    
    public Action ButtonClick
    {
        get => GetValue(ButtonClickProperty);
        set => SetValue(ButtonClickProperty, value);
    }
    
    public string IconKind
    {
        get => GetValue(IconKindProperty);
        set => SetValue(IconKindProperty, value);
    }
    
    public string ButtonText
    {
        get => GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }
    
    public IconButton()
    {
        InitializeComponent();
        DataContext = this;
    }
    
}