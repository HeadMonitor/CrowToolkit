using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CrowEngineProjectManager.Utilities;
using DialogHostAvalonia;
using Newtonsoft.Json;


namespace CrowEngineProjectManager.Views;

public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private bool _correctProjectName;
    private bool _correctProjectPath;
    
    private void ProjectNameTextBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var enteredName = textBox.Text;
        var errorTextBlock = this.FindControl<TextBlock>("ProjectNameErrorText");

        if (errorTextBlock == null) return;
        if (enteredName == null) return;

        if (Utilities.Utilities.IsValidFileName(enteredName))
        {
            errorTextBlock.IsVisible = false;
            _correctProjectName = true;
        }
        else
        {
            errorTextBlock.IsVisible = true;
            _correctProjectName = false;
        }

        // Error Check For Confirm Button
        var confirmButton = this.FindControl<Button>("ConfirmButton");
        confirmButton!.IsEnabled = CorrectText();
    }
    
    public void ProjectPathTextBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var enteredPath = textBox.Text;
        var errorTextBlock = this.FindControl<TextBlock>("ProjectPathErrorText");

        if (errorTextBlock == null) return;

        if (Path.Exists(enteredPath))
        {
            errorTextBlock.IsVisible = false;
            _correctProjectPath = true;
        }
        else
        {
            errorTextBlock.IsVisible = true;
            _correctProjectPath = false;
        }
        
        // Error Check For Confirm Button
        var confirmButton = this.FindControl<Button>("ConfirmButton");
        confirmButton!.IsEnabled = CorrectText();
    }
    
    public async void Browse(object sender, RoutedEventArgs e)
    {
        var selectedPath = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Browse Project Location"
        });

        if (selectedPath.Count <= 0) return;
        
        var projectPathTextBox = this.FindControl<TextBox>("ProjectPathTextBox");
        if (projectPathTextBox != null)
            projectPathTextBox.Text = selectedPath[0].Path.ToString().Replace("file:///", "");
    }

    public void Cancel(object sender, RoutedEventArgs e)
    {
        var mainWindow = this.FindControl<DialogHost>("NewProjectDialogue");

        var dialogHost = mainWindow?.FindControl<DialogHost>("NewProjectDialogue");
        if (dialogHost != null) dialogHost.IsOpen = false;
    }

    public void Confirm(object sender, RoutedEventArgs e)
    {
        var projectNameTextBox = this.FindControl<TextBox>("ProjectNameTextBox");
        var projectName = projectNameTextBox!.Text;
        var projectDescriptionTextBox = this.FindControl<TextBox>("ProjectDescriptionTextBox");
        var projectDescription = projectDescriptionTextBox!.Text;
        var projectPathTextBox = this.FindControl<TextBox>("ProjectPathTextBox");
        var projectPath = projectPathTextBox!.Text;

        // Create The Project Directory
        Directory.CreateDirectory(Path.Combine(projectPath!, projectName!));
        
        // Create The Project Files And Directories
        try
        {
            // Project File
            using var projectFile = File.CreateText(Path.Combine(projectPath!, projectName!, projectName!, ".ctproject"));
            var projectFileData = new
            {
                ProjectName = projectName,
                ProjectDescription = projectDescription,
                CreatedDate = DateTime.Now
            };
            var jsonData = JsonConvert.SerializeObject(projectFileData, Formatting.Indented);
            projectFile.Write(jsonData);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
        
        // Save New Project To List
        var otherProjects = ProjectsListUtilities.LoadFileLocations();
        otherProjects.Add(Path.Combine(projectPath!, projectName!, projectName!, ".ctproject"));
        ProjectsListUtilities.SaveFileLocations(otherProjects);
        
        // Open Editor
        var editorWindow = new CrowEngineUI.Views.MainWindow();
        editorWindow.Show();
        Close(); // Close Project Manager
    }

    /// <summary>
    /// Makes sure that the two possible errors are not triggered before confirming project.
    /// </summary>
    /// <returns>Returns True if both texts are correct.</returns>
    private bool CorrectText()
    {
        return _correctProjectName && _correctProjectPath;
    }
    
}