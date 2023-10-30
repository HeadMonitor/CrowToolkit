
using System;
using Avalonia.Controls;
using CrowEngineProjectManager.Utilities;

namespace CrowEngineProjectManager.Views;

public partial class ProjectList : UserControl
{
    public ProjectList()
    {
        InitializeComponent();

        var projectListGrid = this.FindControl<Grid>("ProjectsList");
        foreach (var projectPath in ProjectsListUtilities.LoadFileLocations())
        {
            projectListGrid!.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            var projectItem = new ProjectItem()
            {
                ProjectName = "",
                ProjectDescription = ""
            };

            Grid.SetRow(projectItem, projectListGrid.RowDefinitions.Count - 1);
            projectListGrid.Children.Add(projectItem);
        }
    }
}