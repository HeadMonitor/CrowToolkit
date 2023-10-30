using Avalonia.Collections;
using Avalonia.Dialogs.Internal;

namespace CrowEngineProjectManager.ViewModels;

public class ProjectListViewModel : AvaloniaDialogsInternalViewModelBase
{
    private AvaloniaList<string> _fileLocations = new();

    public AvaloniaList<string> FileLocations
    {
        get => _fileLocations;
        set => RaiseAndSetIfChanged(ref _fileLocations, value);
    }

    public void AddFileLocation(string location)
    {
        FileLocations.Add(location);
    }

    public void RemoveFileLocation(string location)
    {
        FileLocations.Remove(location);
    }
}