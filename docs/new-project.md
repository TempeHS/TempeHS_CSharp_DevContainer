# Creating new projects

All commands are run from the repository root in the VS Code terminal. Every project lives in
`src/<ProjectName>/` and is listed in the solution file
[`TempeHS.CSharp.slnx`](../TempeHS.CSharp.slnx) so that `dotnet build`, `dotnet test`,
`dotnet publish` and C# Dev Kit's *Solution Explorer* all see it.

Pattern:

```bash
dotnet new <template> -o src/<ProjectName> -n <ProjectName>   # 1. create the project
dotnet sln TempeHS.CSharp.slnx add src/<ProjectName>           # 2. add it to the solution
dotnet run --project src/<ProjectName>                         # 3. run it
```

`dotnet new list` shows every installed template.

## Console app

```bash
dotnet new console -o src/MyTool -n MyTool
dotnet sln TempeHS.CSharp.slnx add src/MyTool
dotnet run --project src/MyTool
```

Runs on Windows, Linux and macOS. Output appears in the terminal.

## Desktop app (Avalonia) – cross-platform GUI

```bash
dotnet new avalonia.app -o src/MyApp -n MyApp        # code-behind style, like the Hello World sample
# or
dotnet new avalonia.mvvm -o src/MyApp -n MyApp       # MVVM style with CommunityToolkit.Mvvm
dotnet sln TempeHS.CSharp.slnx add src/MyApp
dotnet run --project src/MyApp                        # window opens in the virtual desktop
```

Templates are provided by `Avalonia.Templates` (installed by `setup.sh`). If `avalonia.app` is
unknown run `dotnet new install Avalonia.Templates`.

Recommended: copy the Wine-detection block from
[`src/HelloWorld.Desktop/Program.cs`](../src/HelloWorld.Desktop/Program.cs) into your new
`Program.cs` so the Windows `.exe` renders when tested under Wine, and copy the
`<PropertyGroup Condition="'$(RuntimeIdentifier)' != ''">` block from
[`HelloWorld.Desktop.csproj`](../src/HelloWorld.Desktop/HelloWorld.Desktop.csproj) so publishing
produces a single self-contained executable.

Add more windows / controls with the item templates (run inside the project folder):

```bash
cd src/MyApp
dotnet new avalonia.window -n SettingsWindow
dotnet new avalonia.usercontrol -n ScoreBoard
```

## Class library (shared code)

```bash
dotnet new classlib -o src/MyApp.Core -n MyApp.Core
dotnet sln TempeHS.CSharp.slnx add src/MyApp.Core
dotnet add src/MyApp reference src/MyApp.Core     # let MyApp use it
```

Put game logic, data models, file handling etc. in a library so both a console app and a desktop
app – and your tests – can share it.

## Unit tests (xUnit)

```bash
dotnet new xunit -o tests/MyApp.Tests -n MyApp.Tests
dotnet sln TempeHS.CSharp.slnx add tests/MyApp.Tests
dotnet add tests/MyApp.Tests reference src/MyApp.Core
dotnet test TempeHS.CSharp.slnx
```

Tests also show up in VS Code's **Testing** view (flask icon) via C# Dev Kit, and run in CI.

## Windows Forms / WPF (Windows-only GUI)

These frameworks only run on Windows, but thanks to `EnableWindowsTargeting=true` (set in the
container) they can be **compiled** here:

```bash
dotnet new winforms -o src/MyWinFormsApp -n MyWinFormsApp     # or: dotnet new wpf
dotnet sln TempeHS.CSharp.slnx add src/MyWinFormsApp
dotnet build src/MyWinFormsApp
dotnet publish src/MyWinFormsApp -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

Limitations:

- `dotnet run` and **F5** will not work on Linux – the app can only *run* on Windows. WinForms/WPF
  apps generally do **not** start under Wine either; test the published `.exe` on a Windows PC or
  via the GitHub Actions Windows runner.
- There is no designer in VS Code; you write the UI in code (WinForms) or XAML (WPF) by hand.
- `dotnet build TempeHS.CSharp.slnx` on the Linux CI runner needs `-p:EnableWindowsTargeting=true`
  (or add `<EnableWindowsTargeting>true</EnableWindowsTargeting>` to the `.csproj`).

For a new GUI project prefer **Avalonia** – it looks and feels the same, uses the same XAML
concepts, and runs everywhere including the virtual desktop.

## Adding NuGet packages

```bash
dotnet add src/MyApp package Newtonsoft.Json             # latest version
dotnet add src/MyApp package Serilog --version 4.2.0      # specific version
dotnet list src/MyApp package                             # what's installed
```

## Removing a project

```bash
dotnet sln TempeHS.CSharp.slnx remove src/HelloWorld.Console
rm -rf src/HelloWorld.Console
```

Then delete the matching entries in [`.vscode/tasks.json`](../.vscode/tasks.json) and
[`.vscode/launch.json`](../.vscode/launch.json).

## Adding the project to VS Code tasks and debugging

- **Tasks** – `dotnet build/publish TempeHS.CSharp.slnx` already covers every project in the
  solution. Add a `Run:` task for your project by copying one in
  [`.vscode/tasks.json`](../.vscode/tasks.json) and changing the `--project` path.
- **Debugging (F5)** – copy a configuration in [`.vscode/launch.json`](../.vscode/launch.json)
  and change `program` to `${workspaceFolder}/src/MyApp/bin/Debug/net10.0/MyApp.dll` and `cwd`
  to the project folder.
