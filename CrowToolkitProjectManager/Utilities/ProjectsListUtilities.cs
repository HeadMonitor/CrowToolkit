using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CrowEngineProjectManager.Utilities;

public abstract class ProjectsListUtilities
{

    public static List<string> LoadFileLocations()
    {
        var appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (!File.Exists(Path.Combine(appDataFolderPath, "CrowToolkit", "ProjectList.json"))) return new List<string>();
        
        var json = File.ReadAllText(Path.Combine(appDataFolderPath, "CrowToolkit", "ProjectList.json"));
        return JsonSerializer.Deserialize<List<string>>(json) ?? throw new InvalidOperationException();
    }

    public static void SaveFileLocations(List<string> fileLocations)
    {
        var appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var filePath = Path.Combine(appDataFolderPath, "CrowToolkit", "ProjectList.json");
        
        var directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
            if (directoryPath != null)
                Directory.CreateDirectory(directoryPath);

        // Serialize
        var json = JsonSerializer.Serialize(fileLocations);
        File.WriteAllText(filePath, json);
    }
}