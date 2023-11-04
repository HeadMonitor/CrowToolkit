using System;
using Avalonia;
using Avalonia.Controls;

namespace CrowEngineProjectManager.Elements
{
    public partial class LinkButton : UserControl
    {
        public static readonly StyledProperty<Func<string>> ButtonClickProperty =
            AvaloniaProperty.Register<IconButton, Func<string>>(nameof(ButtonClick));

        public static readonly StyledProperty<string> IconKindProperty =
            AvaloniaProperty.Register<IconButton, string>(nameof(IconKind));

        public static readonly StyledProperty<string> ButtonTextProperty =
            AvaloniaProperty.Register<IconButton, string>(nameof(ButtonText));

        public Func<string> ButtonClick
        {
            get => GetValue(ButtonClickProperty);
            set => SetValue(ButtonClickProperty, value);
        }

        public string IconKind
        {
            get => GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public string ButtonText
        {
            get => GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public LinkButton()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}