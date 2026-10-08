using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

using Base.Helpers;

namespace Base.Windows;

public partial class MainWindow : Window
{
    private static readonly TimeSpan _tabTransitionDuration = TimeSpan.FromMilliseconds(180);
    private static readonly string[] _scripts =
    [
        ".run.ps1",
        "build.ps1",
        "buildUpdater.ps1",
        "buildInstaller.ps1",
        "prePush.ps1",
        "newVersion.ps1",
        "updateReadme.ps1",
        "push.ps1",
        "pushRelease.ps1"
    ];

    private readonly string _installPath = AppContext.BaseDirectory.TrimEnd(
        Path.DirectorySeparatorChar,
        Path.AltDirectorySeparatorChar);
    private AppPreferences _preferences = AppPreferences.Defaults;
    private FrameworkElement? _activePage;
    private int _pageTransition;

    public MainWindow()
    {
        InitializeComponent();
        ScriptList.ItemsSource = _scripts;
        InitializeAboutPage();
        RestorePreferences();
        WindowLocationStore.Restore(this);
        SourceInitialized += (_, _) => WindowsTitleBarTheme.ApplyImmersiveDarkMode(this);
    }

    private void InitializeAboutPage()
    {
        AboutTitleText.Text = $"base ({GetPlatformLabel()})";
        AboutVersionText.Text = $"v{ReadVersion()}";
        AboutInstallPathText.Text = _installPath;
        AboutSettingsPathText.Text = AppPreferencesStore.Folder;
    }

    private static string ReadVersion()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Version");
        return File.ReadLines(path).First(line => !string.IsNullOrWhiteSpace(line)).Trim().TrimStart('v', 'V');
    }

    private static string GetPlatformLabel() => RuntimeInformation.ProcessArchitecture switch
    {
        Architecture.X86 => "x86",
        Architecture.X64 => "x64",
        Architecture.Arm64 => "ARM64",
        var architecture => architecture.ToString()
    };

    private void RestorePreferences()
    {
        _preferences = AppPreferencesStore.Load();
        TerminalComboBox.SelectedIndex = (int)_preferences.Terminal;
        PageTabs.SelectedItem = PageTabs.Items.OfType<TabItem>()
            .First(tab => PageName(tab) == _preferences.SelectedPage);
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTabForegrounds(false);
        UpdateTabRowLayout(false);
        ShowPage(false);
    }

    // Scripts

    private void ScriptButton_Click(object sender, RoutedEventArgs e)
    {
        RunScript((string)((Button)sender).Content);
    }

    private void RunScript(string fileName)
    {
        var root = AppContext.BaseDirectory;
        while (Directory.GetParent(root) is { } parent &&
               !File.Exists(Path.Combine(root, "base.sln")))
            root = parent.FullName;

        var path = Path.Combine(root, ".scripts", fileName);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Missing script: {path}", path);

        var wezterm = _preferences.Terminal == TerminalMode.Auto ? FindWezterm() : null;
        ProcessStartInfo startInfo;
        if (wezterm is not null)
        {
            startInfo = new ProcessStartInfo
            {
                FileName = wezterm,
                UseShellExecute = false,
                WorkingDirectory = root
            };
            startInfo.Environment["SCRIPT_OWN_PANE"] = "1";
            startInfo.ArgumentList.Add("start");
            startInfo.ArgumentList.Add("--cwd");
            startInfo.ArgumentList.Add(root);
            startInfo.ArgumentList.Add("--");
            startInfo.ArgumentList.Add("pwsh");
        }
        else
        {
            startInfo = new ProcessStartInfo
            {
                FileName = "pwsh",
                UseShellExecute = true,
                WorkingDirectory = root,
                WindowStyle = ProcessWindowStyle.Normal
            };
        }
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NoExit");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(path);
        Process.Start(startInfo);
    }

    private static string? FindWezterm()
    {
        foreach (var dir in (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
                     .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            try
            {
                var exe = Path.Combine(dir.Trim().Trim('"'), "wezterm.exe");
                if (File.Exists(exe))
                    return exe;
            }
            catch (ArgumentException)
            {
            }
        }

        return null;
    }

    // Settings

    private void TerminalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _preferences = _preferences with { Terminal = (TerminalMode)TerminalComboBox.SelectedIndex };
    }

    // Tabs and pages

    private void PageTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // ComboBox selection events bubble through the TabControl; only react to tab changes.
        if (e.Source != PageTabs)
            return;

        UpdateTabForegrounds(true);
        UpdateTabRowLayout(true);
        if (IsLoaded)
            ShowPage(true);
    }

    private void PageTabs_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateTabRowLayout(false);
    }

    private void UpdateTabRowLayout(bool animate)
    {
        if (PageTabs.SelectedIndex < 0 || PageTabs.Items.Count == 0)
            return;

        var headerHost = PageTabs.Template.FindName("HeaderHost", PageTabs) as FrameworkElement;
        var dividerLayer = PageTabs.Template.FindName("TabDividers", PageTabs) as Panel;
        var indicator = PageTabs.Template.FindName("SelectedTabIndicator", PageTabs) as Border;
        var transform = PageTabs.Template.FindName("SelectedTabIndicatorTransform", PageTabs) as TranslateTransform;
        if (headerHost is null || dividerLayer is null || indicator is null || transform is null || headerHost.ActualWidth <= 0)
            return;

        var tabCount = PageTabs.Items.Count;
        var tabWidth = headerHost.ActualWidth / tabCount;

        indicator.Width = tabWidth;
        UpdateTabDividers(dividerLayer, tabWidth, tabCount);
        if (animate)
        {
            transform.BeginAnimation(
                TranslateTransform.XProperty,
                new DoubleAnimation(PageTabs.SelectedIndex * tabWidth, _tabTransitionDuration)
                {
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                },
                HandoffBehavior.SnapshotAndReplace);
        }
        else
        {
            transform.BeginAnimation(TranslateTransform.XProperty, null);
            transform.X = PageTabs.SelectedIndex * tabWidth;
        }
    }

    private static void UpdateTabDividers(Panel dividerLayer, double tabWidth, int tabCount)
    {
        dividerLayer.Children.Clear();
        var inset = (Thickness)dividerLayer.FindResource("TabDividerInset");
        var brush = (Brush)dividerLayer.FindResource("BorderSubtleBrush");
        for (var index = 1; index < tabCount; index++)
        {
            dividerLayer.Children.Add(new Border
            {
                Width = 1,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(tabWidth * index, inset.Top, inset.Right, inset.Bottom),
                Background = brush,
                SnapsToDevicePixels = true,
                IsHitTestVisible = false
            });
        }
    }

    private void ShowPage(bool animate)
    {
        FrameworkElement? nextPage = (PageTabs.SelectedItem as TabItem)?.Tag switch
        {
            AppPreferences.ScriptsPage => ScriptsPage,
            AppPreferences.SettingsPage => SettingsPage,
            AppPreferences.AboutPage => AboutPage,
            _ => null
        };
        if (nextPage is null || ReferenceEquals(nextPage, _activePage))
            return;

        var previousPage = _activePage;
        _activePage = nextPage;
        var transition = ++_pageTransition;

        nextPage.BeginAnimation(OpacityProperty, null);
        nextPage.Visibility = Visibility.Visible;
        if (!animate || previousPage is null)
        {
            nextPage.Opacity = 1;
            HideInactivePage(ScriptsPage, nextPage);
            HideInactivePage(SettingsPage, nextPage);
            HideInactivePage(AboutPage, nextPage);
            return;
        }

        previousPage.BeginAnimation(OpacityProperty, null);
        previousPage.Visibility = Visibility.Visible;
        previousPage.Opacity = 1;
        nextPage.Opacity = 0;

        var fadeOut = new DoubleAnimation(1, 0, _tabTransitionDuration);
        var fadeIn = new DoubleAnimation(0, 1, _tabTransitionDuration);
        fadeIn.Completed += (_, _) =>
        {
            // A newer tab change owns the pages now.
            if (transition != _pageTransition)
                return;

            previousPage.Visibility = Visibility.Collapsed;
            previousPage.Opacity = 0;
        };
        previousPage.BeginAnimation(OpacityProperty, fadeOut);
        nextPage.BeginAnimation(OpacityProperty, fadeIn);
    }

    private static void HideInactivePage(FrameworkElement page, FrameworkElement activePage)
    {
        if (ReferenceEquals(page, activePage))
            return;

        page.BeginAnimation(OpacityProperty, null);
        page.Opacity = 0;
        page.Visibility = Visibility.Collapsed;
    }

    private static string PageName(TabItem tab) => tab.Tag.ToString()!;

    private void UpdateTabForegrounds(bool animate)
    {
        var selectedColor = ResourceColor("PrimaryTextBrush");
        var unselectedColor = ResourceColor("TertiaryTextBrush");
        foreach (var tab in PageTabs.Items.OfType<TabItem>())
        {
            // Style brushes are frozen; give each tab its own animatable brush.
            if (tab.Foreground is not SolidColorBrush foreground)
                continue;

            var brush = foreground.IsFrozen ? foreground.Clone() : foreground;
            tab.Foreground = brush;
            AnimateColor(brush, tab.IsSelected ? selectedColor : unselectedColor, animate);
        }
    }

    private Color ResourceColor(string key) => ((SolidColorBrush)FindResource(key)).Color;

    private static void AnimateColor(SolidColorBrush brush, Color color, bool animate)
    {
        brush.BeginAnimation(
            SolidColorBrush.ColorProperty,
            animate
                ? new ColorAnimation(color, _tabTransitionDuration)
                {
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                }
                : null);
        if (!animate)
            brush.Color = color;
    }

    // About

    private void AboutOpenInstallLocation_Click(object sender, RoutedEventArgs e)
    {
        OpenFolder(_installPath);
    }

    private void AboutOpenSettingsLocation_Click(object sender, RoutedEventArgs e)
    {
        Directory.CreateDirectory(AppPreferencesStore.Folder);
        OpenFolder(AppPreferencesStore.Folder);
    }

    private static void OpenFolder(string path)
    {
        Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
    }

    private void AboutHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true });
        e.Handled = true;
    }

    private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        AppPreferencesStore.Save(_preferences with { SelectedPage = PageName((TabItem)PageTabs.SelectedItem) });
        WindowLocationStore.Save(this);
    }
}
