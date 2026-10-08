using System.Windows;

using Base.Helpers;

namespace BaseUpdater.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SourceInitialized += (_, _) => WindowsTitleBarTheme.ApplyImmersiveDarkMode(this);
    }
}
