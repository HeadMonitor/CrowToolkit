
using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using CrowEngineProjectManager.Utilities;
using Newtonsoft.Json;

namespace CrowEngineProjectManager.Views;

public partial class ProjectList : UserControl
{
    public ProjectList()
    {
        InitializeComponent();

        var projectListGrid = this.FindControl<Grid>("ProjectsList");
        foreach (var projectPath in ProjectsListUtilities.LoadFileLocations())
        {
            List<string>? items;
            try
            {
                var projectData = File.ReadAllText(projectPath);
                items = JsonConvert.DeserializeObject<List<string>>(projectData);
            }
            catch (Exception e) // Project File Defaults
            {
                items = new List<string>()
                {
                    "Missing Project",
                    "",
                    projectPath
                };
            }
            
            projectListGrid!.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            var projectItem = new ProjectItem()
            {
                ProjectName = items![0],
                ProjectDescription = items![1],
                ProjectPath = items[2]
            };

            Grid.SetRow(projectItem, projectListGrid.RowDefinitions.Count - 1);
            projectListGrid.Children.Add(projectItem);
        }
    }
}