using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CrowEngineProjectManager.ViewModels;

namespace CrowEngineProjectManager;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new Views.MainWindow()
            {
                DataContext = new MainWindowTabsViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}