namespace CrowEngineUI.ViewModels;

public class TabItemViewModel 
{
    public string Header { get; set; }
        
    public string SimpleContent { get; set; }


    public override string ToString() => Header;
}