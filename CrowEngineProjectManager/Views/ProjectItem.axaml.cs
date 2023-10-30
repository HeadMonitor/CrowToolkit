using Avalonia;
using Avalonia.Controls;

namespace CrowEngineProjectManager.Views;

public partial class ProjectItem : UserControl
{
    
    public static readonly StyledProperty<string> ProjectNameProperty =
        AvaloniaProperty.Register<ProjectItem, string>(nameof(ProjectName), defaultValue: "Project Name");

    public static readonly StyledProperty<string> ProjectDescriptionProperty =
        AvaloniaProperty.Register<ProjectItem, string>(nameof(ProjectDescription), defaultValue: "Project Description");
    
    public static readonly StyledProperty<string> ProjectPathProperty =
        AvaloniaProperty.Register<ProjectItem, string>(nameof(ProjectPath), defaultValue: "Project/Path");
    
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

    public string ProjectPath
    {
        get => GetValue(ProjectPathProperty);
        set => SetValue(ProjectPathProperty, value);
    }

    public ProjectItem()
    {
        InitializeComponent();
        DataContext = this;
    }

}