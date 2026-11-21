# Avalonia basics – how the Hello World desktop app works

[Avalonia UI](https://avaloniaui.net/) is a cross-platform desktop framework for .NET. You describe
the window in **XAML** (an XML dialect, the same idea as WPF) and write the behaviour in a C#
**code-behind** file. The same code produces a native-looking window on Windows, Linux and macOS.

This page walks through every file in [`src/HelloWorld.Desktop`](../src/HelloWorld.Desktop).

## The files

| File                                                               | What it is                                                                                      |
| ------------------------------------------------------------------ | ----------------------------------------------------------------------------------------------- |
| [`HelloWorld.Desktop.csproj`](../src/HelloWorld.Desktop/HelloWorld.Desktop.csproj) | Project file – target framework, NuGet packages, publish settings                |
| [`Program.cs`](../src/HelloWorld.Desktop/Program.cs)               | Entry point (`Main`). Configures and starts Avalonia                                            |
| [`App.axaml`](../src/HelloWorld.Desktop/App.axaml) / `.axaml.cs`   | The application object – global styles/theme and which window to open first                     |
| [`MainWindow.axaml`](../src/HelloWorld.Desktop/MainWindow.axaml)   | **The UI** – what the window looks like                                                         |
| [`MainWindow.axaml.cs`](../src/HelloWorld.Desktop/MainWindow.axaml.cs) | **The behaviour** – what happens when you click the button                                  |
| [`app.manifest`](../src/HelloWorld.Desktop/app.manifest)           | Windows-only metadata (DPI awareness, supported Windows versions). Leave it alone               |

Each `.axaml` file has a matching `.axaml.cs` file. Together they form **one** C# class
(`partial class MainWindow`) – the XAML half is compiled into C# for you at build time.

## 1. `Program.cs` – starting the app

```csharp
[STAThread]
public static void Main(string[] args) => BuildAvaloniaApp()
    .StartWithClassicDesktopLifetime(args);
```

`BuildAvaloniaApp()` tells Avalonia which `App` class to use, to detect the platform (Windows /
X11 / macOS), to bundle the *Inter* font and, in Debug builds, to enable Developer Tools (press
**F12** in the running app). `StartWithClassicDesktopLifetime` runs the app until its last window
is closed.

The `IsRunningUnderWine()` check switches to software rendering *only* when the Windows `.exe` is
running under Wine in the virtual desktop (Wine has no GPU). Real Windows PCs are unaffected.

## 2. `App.axaml` – theme and first window

```xml
<Application.Styles>
    <FluentTheme />
</Application.Styles>
```

`FluentTheme` gives every control a modern Windows-11-style look. Change
`RequestedThemeVariant="Default"` to `"Dark"` or `"Light"` to force a theme instead of following
the OS. In `App.axaml.cs`, `OnFrameworkInitializationCompleted` creates the `MainWindow`.

## 3. `MainWindow.axaml` – the UI

```xml
<Window ... Title="Hello World - TempeHS C#" Width="480" Height="320">
    <StackPanel Margin="24" Spacing="16" VerticalAlignment="Center">
        <TextBlock Text="Hello, World!" FontSize="32" FontWeight="Bold" HorizontalAlignment="Center" />
        <TextBlock x:Name="PlatformText" ... />
        <Button x:Name="ClickMeButton" Content="Click me!" Click="OnClickMe" ... />
        <TextBlock x:Name="CounterText" Text="You have not clicked the button yet." ... />
    </StackPanel>
</Window>
```

Key ideas:

- **Controls are XML elements**; their properties are attributes (`FontSize="32"`).
- A `Window` holds exactly **one** child, so you put a *layout panel* inside it and place the
  controls in the panel. `StackPanel` stacks children vertically (or horizontally with
  `Orientation="Horizontal"`). Other panels: `Grid` (rows/columns), `DockPanel`, `WrapPanel`.
- **`x:Name="PlatformText"`** gives a control a name. The compiler then generates a field
  `PlatformText` in `MainWindow` so the C# code can read/modify it.
- **`Click="OnClickMe"`** connects the button's `Click` event to a method in the code-behind.

Common controls to try: `TextBox` (text input), `CheckBox`, `RadioButton`, `ComboBox`, `Slider`,
`ListBox`, `Image`, `ProgressBar`, `Menu`, `TabControl`. See the
[Avalonia controls reference](https://docs.avaloniaui.net/docs/reference/controls/).

## 4. `MainWindow.axaml.cs` – the behaviour

```csharp
public partial class MainWindow : Window
{
    private int _clickCount;

    public MainWindow()
    {
        InitializeComponent();          // builds the UI from MainWindow.axaml
        PlatformText.Text = $"Running on {RuntimeInformation.OSDescription} ...";
    }

    private void OnClickMe(object? sender, RoutedEventArgs e)
    {
        _clickCount++;
        CounterText.Text = $"You clicked the button {_clickCount} times.";
    }
}
```

- `InitializeComponent()` must be the first thing in the constructor; after it, every `x:Name`d
  control is available as a field.
- Event handlers always have the shape `void Name(object? sender, SomeEventArgs e)`.
- Changing a property such as `CounterText.Text` updates the screen immediately.

## 5. Try it

1. Press **F5** to run the app in the virtual desktop.
2. Change the `Text` of the first `TextBlock`, save, and press **F5** again.
3. Add a `TextBox x:Name="NameBox"` above the button and change `OnClickMe` to show
   `$"Hello, {NameBox.Text}!"`.
4. Set a breakpoint inside `OnClickMe` (click in the gutter next to the line number), press F5,
   and click the button – the debugger pauses and you can inspect `_clickCount`.

## 6. Going further

- **The XAML previewer** – the *Avalonia for VS Code* extension can show a live preview of an
  `.axaml` file: open the file and click the *Show preview* icon (top-right of the editor).
  Build the project once first.
- **Developer Tools** – press **F12** in a Debug build to inspect the visual tree and live-edit
  properties.
- **MVVM** – for anything bigger than a few controls, Avalonia recommends the *Model-View-ViewModel*
  pattern with data binding. `dotnet new avalonia.mvvm` creates a starter project for it.
- **Documentation** – [docs.avaloniaui.net](https://docs.avaloniaui.net/), in particular
  [Get started](https://docs.avaloniaui.net/docs/get-started/) and
  [Basics](https://docs.avaloniaui.net/docs/basics/).
