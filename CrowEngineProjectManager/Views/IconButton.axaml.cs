using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CrowEngineProjectManager.Views;

public partial class IconButton : UserControl
{
    public static readonly StyledProperty<string> IconProperty =
        AvaloniaProperty.Register<IconButton, string>(nameof(Icon));

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<IconButton, string>(nameof(Text));

    public string Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public IconButton()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}