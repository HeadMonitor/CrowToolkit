using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using DialogHostAvalonia;

namespace CrowEngineProjectManager.Views;

public partial class SidePanel : UserControl
{
    public SidePanel()
    {
        InitializeComponent();
    }
    
    private void OnNewProjectButtonClicked(object sender, RoutedEventArgs e)
    {
        var mainWindow = this.GetLogicalAncestors().OfType<MainWindow>().FirstOrDefault();

        var dialogHost = mainWindow?.FindControl<DialogHost>("NewProjectDialogue");
        if (dialogHost != null) dialogHost.IsOpen = true;
    }

    private void ScreenCrowStudiosWebsite(object? sender, RoutedEventArgs e)
    {
       Process.Start(new ProcessStartInfo { FileName = @"https://screencrowstudios.com", UseShellExecute = true });
    }

    private void CrowToolkitWebsite(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo { FileName = @"https://crowtoolkit.screencrowstudios.com", UseShellExecute = true });
    }

    private void GitHubRepository(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo { FileName = @"https://github.com/ScreenCrowStudios/CrowToolkit", UseShellExecute = true });
    }
}