using System.Text.RegularExpressions;

namespace CrowEngineProjectManager.Utilities;

public partial class Utilities
{
    public static bool IsValidFileName(string fileName)
    {
        const string pattern = @"^[^<>:""/\\|?*]+$";
        return MyRegex().IsMatch(fileName);
    }
    [GeneratedRegex("^[^<>:\"/\\\\|?*]+$")]
    private static partial Regex MyRegex();
    
}