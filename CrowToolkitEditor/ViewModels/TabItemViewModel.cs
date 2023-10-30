namespace CrowEngineUI.ViewModels;

public class TabItemViewModel
{
    public required string Header { get; set; }
    public required string SimpleContent { get; set; }

    public override string ToString()
    {
        return Header;
    }
}