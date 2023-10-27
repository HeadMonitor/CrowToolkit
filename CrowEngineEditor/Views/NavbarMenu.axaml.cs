using Avalonia.Controls;
using Avalonia.Interactivity;
using CrowEngineUI.Views.Tools;

namespace CrowEngineUI;

public partial class NavbarMenu : UserControl
{
    public NavbarMenu()
    {
        InitializeComponent();
    }

    /* File Menu */
    
    private void NewProject(object? sender, RoutedEventArgs routedEventArgs)
    {
        
    }

    private void OpenProject(object? sender, RoutedEventArgs routedEventArgs)
    {
        
    }
    
    private void Save(object? sender, RoutedEventArgs routedEventArgs)
    {
        
    }
    
    private void Exit(object? sender, RoutedEventArgs routedEventArgs)
    {
        
    }

    /* Edit Menu */
    
    /* Tools Menu */
    
    private void Compressor(object? sender, RoutedEventArgs routedEventArgs)
    {
        var compressor = new Compressor();

        compressor.Show();
    }
    
    private void Converter(object? sender, RoutedEventArgs routedEventArgs)
    {
        
    }
    
}