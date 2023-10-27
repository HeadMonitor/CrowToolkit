using System.ComponentModel;

namespace CrowEngineProjectManager.ViewModels;

public sealed class IconButtonViewModel : INotifyPropertyChanged
{
    private string icon;
    private string text;

    public string Icon
    {
        get => icon;
        set
        {
            if (icon != value)
            {
                icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }
    }

    public string Text
    {
        get => text;
        set
        {
            if (text != value)
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}