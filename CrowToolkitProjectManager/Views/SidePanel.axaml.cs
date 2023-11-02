using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
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
}