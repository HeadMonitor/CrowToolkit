using Avalonia.Controls;
using Avalonia.Interactivity;
using CrowEngineEditor___Cross_Platform_Desktop_Exporter;

namespace CrowEngineUI.Views;

public partial class Navbar : UserControl
{
    public Navbar()
    {
        InitializeComponent();
    }

    private void Play(object? sender, RoutedEventArgs e)
    {
        var game1 = new Game1();
        game1.Run();
    }
}