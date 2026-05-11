using MudBlazor;

namespace Hellbot.UI.Theme;

public static class HellbotTheme
{
    public static MudTheme Instance { get; } = Create();

    private static MudTheme Create()
    {
        var theme = new MudTheme();

        theme.PaletteDark = new PaletteDark
        {
            Primary = "#818cf8",
            PrimaryContrastText = "#0f172a",
            Secondary = "#94a3b8",
            SecondaryContrastText = "#0f172a",
            Tertiary = "#2dd4bf",
            AppbarBackground = "#0f172a",
            AppbarText = "#f8fafc",
            Background = "#0f172a",
            BackgroundGray = "#1e293b",
            Surface = "#1e293b",
            DrawerBackground = "#020617",
            DrawerText = "#f1f5f9",
            DrawerIcon = "#94a3b8",
            TextPrimary = "#f8fafc",
            TextSecondary = "#cbd5e1",
            TextDisabled = "#64748b",
            ActionDefault = "#94a3b8",
            Divider = "#334155",
            DividerLight = "#1e293b",
            TableLines = "#334155",
            LinesDefault = "#334155",
            LinesInputs = "#475569",
            Success = "#15803d",
            SuccessContrastText = "#ecfdf5",
            Warning = "#fbbf24",
            Error = "#f87171",
            Info = "#60a5fa",
        };

        return theme;
    }
}
