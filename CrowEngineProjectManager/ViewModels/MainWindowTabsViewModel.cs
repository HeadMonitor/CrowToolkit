using System;
using System.Collections.ObjectModel;

namespace CrowEngineProjectManager.ViewModels;

public class MainWindowTabsViewModel
{
    private int _i;

    public MainWindowTabsViewModel()
    {
        TabItems.Add(new TabItemViewModel
        {
            Header = "Fixed Tab",
            SimpleContent = "Fixed Tab content"
        });

        const int count = 10;

        for (var i = 0; i < count; i++)
            TabItems.Add((TabItemViewModel)AddItem());
    }

    public Func<object> NewItemFactory => AddItem;
    public ObservableCollection<TabItemViewModel> TabItems { get; } = new();

    private object AddItem()
    {
        return new TabItemViewModel
        {
            Header = $"Tab {++_i}",
            SimpleContent = $"Tab {_i} content"
        };
    }
}