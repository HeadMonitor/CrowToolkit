using Avalonia;
using Avalonia.Controls;

namespace CrowEngineProjectManager.Views;

public partial class ProjectItem : UserControl
{
    
    public static readonly StyledProperty<string> ProjectNameProperty =
        AvaloniaProperty.Register<ProjectItem, string>(nameof(ProjectName), defaultValue: "Project Name");

    public static readonly StyledProperty<string> ProjectDescriptionProperty =
        AvaloniaProperty.Register<ProjectItem, string>(nameof(ProjectDescription), defaultValue: "Project Description");
    
    public string ProjectName
    {
        get => GetValue(ProjectNameProperty);
        set => SetValue(ProjectNameProperty, value);
    }

    public string ProjectDescription
    {
        get => GetValue(ProjectDescriptionProperty);
        set => SetValue(ProjectDescriptionProperty, value);
    }

    public ProjectItem()
    {
        InitializeComponent();
        DataContext = this;
    }

}