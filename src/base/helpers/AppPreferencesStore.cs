using System.IO;
using System.Text.Json;

namespace Base.Helpers;

internal static class AppPreferencesStore
{
    public static readonly string Folder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "base");

    private static readonly string _path = Path.Combine(Folder, "preferences.json");

    public static AppPreferences Load()
    {
        try
        {
            if (!File.Exists(_path))
                return AppPreferences.Defaults;

            var preferences = JsonSerializer.Deserialize<AppPreferences>(File.ReadAllText(_path));
            return preferences?.Normalize() ?? AppPreferences.Defaults;
        }
        catch (JsonException) { return AppPreferences.Defaults; }
        catch (IOException) { return AppPreferences.Defaults; }
        catch (UnauthorizedAccessException) { return AppPreferences.Defaults; }
    }

    public static void Save(AppPreferences preferences)
    {
        try
        {
            Directory.CreateDirectory(Folder);
            File.WriteAllText(_path, JsonSerializer.Serialize(preferences.Normalize()));
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

internal enum TerminalMode
{
    Auto,
    Pwsh
}

internal sealed record AppPreferences(string? SelectedPage = null, TerminalMode Terminal = TerminalMode.Auto)
{
    public const string ScriptsPage = "Scripts";
    public const string SettingsPage = "Settings";
    public const string AboutPage = "About";

    public static AppPreferences Defaults { get; } = new(ScriptsPage);

    public AppPreferences Normalize() => new(
        SelectedPage is ScriptsPage or SettingsPage or AboutPage ? SelectedPage : ScriptsPage,
        Enum.IsDefined(Terminal) ? Terminal : TerminalMode.Auto);
}
