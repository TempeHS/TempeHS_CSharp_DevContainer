using System.Runtime.InteropServices;
using Avalonia;

namespace HelloWorld.Desktop;

internal static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by the visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        var builder = AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

        // Wine (used to smoke-test the Windows .exe inside the virtual desktop) has no GPU,
        // so the window would stay blank with the default GPU renderer. Real Windows PCs are
        // unaffected and keep Avalonia's default rendering mode.
        if (IsRunningUnderWine())
        {
            builder = builder.With(new Win32PlatformOptions
            {
                RenderingMode = [Win32RenderingMode.Software],
            });
        }

        return builder;
    }

    private static bool IsRunningUnderWine()
        => OperatingSystem.IsWindows()
           && NativeLibrary.TryLoad("ntdll.dll", out var ntdll)
           && NativeLibrary.TryGetExport(ntdll, "wine_get_version", out _);
}
