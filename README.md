# TempeHS C# DevContainer

A ready-to-go GitHub Codespace / Dev Container for writing **C# desktop and console applications**
with **.NET 10** and compiling them for **Windows**, **Linux** and **macOS** from a single
environment – no Windows PC or Mac required.

| Target  | How it is built                                                  | How you test it                                           |
| ------- | ---------------------------------------------------------------- | --------------------------------------------------------- |
| Linux   | `dotnet publish -r linux-x64`                                    | Run it in the terminal or in the **virtual desktop**      |
| Windows | `dotnet publish -r win-x64` (cross-compiled, single `.exe`)      | Run it with **Wine** inside the virtual desktop, or on CI |
| macOS   | `dotnet publish -r osx-arm64` / `osx-x64` (cross-compiled)       | Copy to a Mac, or download the GitHub Actions artifact    |

Desktop apps use [Avalonia UI](https://avaloniaui.net/) – a XAML framework very similar to WPF
that runs on all three operating systems. The container also ships a **virtual desktop** (noVNC in
your browser) so you can see windowed apps – including Windows `.exe` files running under Wine –
without leaving the Codespace.

---

## Contents

1. [Quick start](#1-quick-start)
2. [What's installed](#2-whats-installed)
3. [Project layout](#3-project-layout)
4. [Building and running](#4-building-and-running)
   - [Build (Debug / Release)](#41-build)
   - [Run on Linux](#42-run-on-linux)
   - [Publish for Linux, Windows and macOS](#43-publish-for-linux-windows-and-macos)
   - [Run the Windows .exe with Wine](#44-run-the-windows-exe-with-wine)
5. [The virtual desktop (virtual window)](#5-the-virtual-desktop-virtual-window)
6. [Debugging](#6-debugging)
7. [Using this template for your own project](#7-using-this-template-for-your-own-project)
8. [Continuous integration](#8-continuous-integration)
9. [Troubleshooting](#9-troubleshooting)
10. [Further documentation](#10-further-documentation)

---

## 1. Quick start

1. Click **Code ▸ Codespaces ▸ Create codespace on main** (or open the folder in VS Code and
   choose **Reopen in Container**).
2. Wait for the container to build. The first build takes a few minutes while
   [`.devcontainer/setup.sh`](.devcontainer/setup.sh) installs Wine, the Avalonia templates and
   restores NuGet packages.
3. Press **F5**. The *Hello World* desktop app is built and launched – it appears in the
   **virtual desktop** (a browser tab on port 6080 opens automatically; password `vscode`).
   Click the button and watch the counter update.
4. Open a terminal (`` Ctrl+` ``) and try the console version and a Windows build:

   ```bash
   # Console app on Linux
   dotnet run --project src/HelloWorld.Console

   # Build a Windows .exe for every project, then run it under Wine
   dotnet publish TempeHS.CSharp.slnx -c Release -r win-x64
   wine publish/win-x64/HelloWorld.Console.exe
   ```

   Expected output:

   ```text
   Hello, World!
   Running on: Ubuntu 24.04.5 LTS (X64)                    ← Linux build
   Runtime:    .NET 10.0.x

   Hello, World!
   Running on: Microsoft Windows 10.0.19043 (X64)          ← the .exe under Wine
   Runtime:    .NET 10.0.x
   ```

---

## 2. What's installed

Everything below is installed automatically by the devcontainer:

| Tool                                                 | Purpose                                                        |
| ---------------------------------------------------- | -------------------------------------------------------------- |
| .NET 10 SDK (`dotnet`)                               | Compiler, build system, package manager, test runner           |
| Avalonia project templates (`dotnet new avalonia.*`) | Create new cross-platform desktop apps                         |
| Avalonia / SkiaSharp native libraries (X11, fontconfig, Mesa) | Lets Avalonia windows draw on the virtual desktop     |
| Wine (64-bit)                                        | Run the Windows `.exe` inside the container                    |
| `desktop-lite` feature (Fluxbox + TigerVNC + noVNC)  | The browser-based **virtual desktop**                          |
| `xterm`, `file`, GitHub CLI (`gh`)                   | Utilities                                                      |
| `EnableWindowsTargeting=true` (environment variable) | Allows WinForms / WPF projects to be *compiled* on Linux       |

**VS Code extensions** (see [`.vscode/extensions.json`](.vscode/extensions.json)):
C# (debugger + IntelliSense), C# Dev Kit (Solution Explorer, test explorer), .NET Install Tool,
Avalonia for VS Code (XAML IntelliSense + previewer), EditorConfig, GitLens, GitHub Pull Requests,
GitHub Actions.

---

## 3. Project layout

```text
.
├── .devcontainer/
│   ├── devcontainer.json        # Container image, features, ports, env vars, extensions
│   └── setup.sh                 # Installs Wine, native libs, Avalonia templates; restores NuGet
├── .github/workflows/build.yml  # CI: build, format check, test, publish for all 3 OSes
├── .vscode/
│   ├── extensions.json          # Recommended extensions
│   ├── settings.json            # C# / formatting / virtual-desktop settings
│   ├── tasks.json               # Build, run, publish tasks (Ctrl+Shift+B)
│   └── launch.json              # Debug configurations (F5)
├── docs/
│   ├── avalonia-basics.md       # How the Hello World desktop app works (XAML, code-behind)
│   ├── new-project.md           # Creating console, desktop, library, test, WinForms/WPF projects
│   └── publishing.md            # Self-contained builds, runtime identifiers, icons, versions
├── src/
│   ├── HelloWorld.Console/      # Console "Hello, World!" (Program.cs)
│   └── HelloWorld.Desktop/      # Avalonia desktop "Hello, World!" (MainWindow.axaml + .cs)
├── Directory.Build.props        # Shared MSBuild settings (publish output → publish/<rid>/)
├── TempeHS.CSharp.slnx          # Solution file – lists the projects
├── global.json                  # Pins the .NET SDK major version (10.0)
├── .editorconfig                # Code style (enforced by `dotnet format`)
└── .gitignore
```

Build output goes to `src/<project>/bin/`, published apps go to `publish/<runtime>/`, e.g.
`publish/win-x64/HelloWorld.Desktop.exe`. Both folders are git-ignored and hidden in the Explorer.

---

## 4. Building and running

You can drive everything from:

- **The terminal** – `dotnet build`, `dotnet run`, `dotnet publish`
- **VS Code tasks** – `Ctrl+Shift+B` (or *Terminal ▸ Run Task…*) and pick a task
- **C# Dev Kit** – right-click a project in the *Solution Explorer* view

| Task (`Ctrl+Shift+B`)                                  | Terminal equivalent                                         |
| ------------------------------------------------------ | ----------------------------------------------------------- |
| `Build: Solution (Debug)` *(default)*                  | `dotnet build TempeHS.CSharp.slnx`                          |
| `Build: Solution (Release)`                            | `dotnet build TempeHS.CSharp.slnx -c Release`               |
| `Run: Desktop app (Linux, in virtual desktop)`         | `dotnet run --project src/HelloWorld.Desktop`               |
| `Run: Console app (Linux)`                             | `dotnet run --project src/HelloWorld.Console`               |
| `Publish: Linux x64 (self-contained)`                  | `dotnet publish TempeHS.CSharp.slnx -c Release -r linux-x64` |
| `Publish: Windows x64 (self-contained .exe)`           | `dotnet publish TempeHS.CSharp.slnx -c Release -r win-x64`  |
| `Publish: macOS (Apple Silicon + Intel)`               | `dotnet publish … -r osx-arm64` and `… -r osx-x64`          |
| `Publish: All (Linux + Windows + macOS)`               | all of the above                                            |
| `Run: Windows Desktop .exe (Wine, in virtual desktop)` | `wine publish/win-x64/HelloWorld.Desktop.exe`               |
| `Run: Windows Console .exe (Wine)`                     | `wine publish/win-x64/HelloWorld.Console.exe`               |
| `Test: Run all tests`                                  | `dotnet test TempeHS.CSharp.slnx`                           |
| `Clean: remove bin, obj and publish folders`           | `dotnet clean … && rm -rf publish src/*/bin src/*/obj`      |

### 4.1 Build

```bash
dotnet build TempeHS.CSharp.slnx              # Debug build of every project
dotnet build TempeHS.CSharp.slnx -c Release   # Optimised build
```

A plain build is *framework-dependent* – it produces a small `.dll` that runs with the .NET
runtime installed in the container. That is what `dotnet run` and the debugger use.

### 4.2 Run on Linux

```bash
dotnet run --project src/HelloWorld.Console   # output appears in the terminal
dotnet run --project src/HelloWorld.Desktop   # window appears in the virtual desktop
```

The VS Code terminal already has `DISPLAY=:1`, so GUI apps show up in the virtual desktop
automatically (see [section 5](#5-the-virtual-desktop-virtual-window)).

### 4.3 Publish for Linux, Windows and macOS

Publishing creates the file you actually give to someone else. The projects are configured
(see [`HelloWorld.Desktop.csproj`](src/HelloWorld.Desktop/HelloWorld.Desktop.csproj)) so that
publishing with a runtime identifier (`-r`) produces **one self-contained executable** that runs
on a clean PC **without .NET installed**:

```bash
dotnet publish TempeHS.CSharp.slnx -c Release -r linux-x64    # → publish/linux-x64/HelloWorld.Desktop
dotnet publish TempeHS.CSharp.slnx -c Release -r win-x64      # → publish/win-x64/HelloWorld.Desktop.exe
dotnet publish TempeHS.CSharp.slnx -c Release -r osx-arm64    # → publish/osx-arm64/HelloWorld.Desktop  (Apple Silicon)
dotnet publish TempeHS.CSharp.slnx -c Release -r osx-x64      # → publish/osx-x64/HelloWorld.Desktop    (Intel Mac)
```

Verify what you built:

```bash
file publish/win-x64/HelloWorld.Desktop.exe
# → PE32+ executable (GUI) x86-64, for MS Windows
file publish/osx-arm64/HelloWorld.Desktop
# → Mach-O 64-bit arm64 executable
```

To get the file onto your computer, right-click it in the VS Code Explorer and choose
**Download**. (Build output is hidden in the Explorer, but the `publish/` folder is not.)
Expect roughly 40–50 MB per executable: it contains the whole .NET runtime so nothing has to be
installed on the target machine. See [`docs/publishing.md`](docs/publishing.md) for smaller
framework-dependent builds, icons, version numbers and other options.

### 4.4 Run the Windows .exe with Wine

Wine translates Windows API calls to Linux so you can smoke-test the Windows build right here:

```bash
# Console apps – output appears in the VS Code terminal
wine publish/win-x64/HelloWorld.Console.exe

# GUI apps appear in the virtual desktop
wine publish/win-x64/HelloWorld.Desktop.exe
```

The first `wine` launch after the container starts takes ~10–20 s while Wine initialises. Wine is
a good smoke test but not a substitute for a real Windows machine – the CI workflow
([section 8](#8-continuous-integration)) runs the console `.exe` on a real Windows runner, and
you should always do a final check on Windows before shipping.

> **Note:** Wine has no GPU, so Avalonia's default (hardware-accelerated) Windows renderer draws a
> blank window under Wine. [`Program.cs`](src/HelloWorld.Desktop/Program.cs) detects Wine and
> switches to software rendering only in that case; real Windows PCs are unaffected. Keep that
> snippet when you build your own Avalonia apps.

---

## 5. The virtual desktop (virtual window)

Codespaces have no screen, so the container runs a lightweight X11 desktop (Fluxbox) behind a VNC
server and exposes it in your browser via noVNC. Any program that opens a window – an Avalonia
app, or a Windows `.exe` running under Wine – appears there.

### 5.1 Open the virtual desktop

1. Open the **Ports** panel in VS Code (*View ▸ Ports*, or the Ports tab next to the terminal).
2. Find port **6080 – Virtual Desktop (noVNC)** and click the 🌐 globe icon
   (it may also open automatically when the Codespace starts).
3. Click **Connect** in the noVNC page and enter the password **`vscode`**.

You now have a full desktop in a browser tab. Right-click the desktop background for the Fluxbox
menu (it includes a terminal).

> Tip: drag the browser tab to a second monitor, or use VS Code's **Simple Browser**
> (`Ctrl+Shift+P` ▸ *Simple Browser: Show* ▸ `http://localhost:6080`) to see it side-by-side
> with your code.

### 5.2 Run apps in the virtual desktop

The VS Code terminal, tasks and debugger all set `DISPLAY=:1`, so GUI apps just appear:

```bash
dotnet run --project src/HelloWorld.Desktop            # Linux build
wine publish/win-x64/HelloWorld.Desktop.exe            # Windows build under Wine
xterm -hold -e ./publish/linux-x64/HelloWorld.Console & # console app in a window on the desktop
```

### 5.3 Change the desktop resolution

The desktop defaults to 1440×768. To change it, set `VNC_RESOLUTION` before the desktop starts,
e.g. add to `containerEnv` in [`.devcontainer/devcontainer.json`](.devcontainer/devcontainer.json):

```json
"VNC_RESOLUTION": "1920x1080x24"
```

then **Rebuild Container**.

---

## 6. Debugging

- Press **F5** (*Debug: Desktop app (Avalonia)*). The solution is built first, then the app is
  launched under the .NET debugger with breakpoints, watch, call stack and hot reload.
  The window appears in the virtual desktop.
- Pick **Debug: Console app** from the *Run and Debug* drop-down to debug the console sample. Its
  output appears in the integrated terminal.
- **Attach to running .NET process** lets you attach to something you started yourself.
- In a Debug build of the Avalonia app, press **F12** inside the window to open Avalonia's
  **Developer Tools** – an inspector for the visual tree, properties and styles.

Windows builds cannot be source-debugged here; debug the Linux build (same code) instead and use
Wine only to confirm the `.exe` runs.

Configurations live in [`.vscode/launch.json`](.vscode/launch.json). When you add a new project,
copy one of the entries and change the `program` path.

---

## 7. Using this template for your own project

1. Click **Use this template** on GitHub, or copy `.devcontainer/`, `.vscode/`, `.github/`,
   `Directory.Build.props`, `global.json`, `.editorconfig` and `.gitignore` into your repo.
2. Create your project – see [`docs/new-project.md`](docs/new-project.md) for console, desktop,
   library, test, WinForms and WPF projects, e.g.

   ```bash
   dotnet new avalonia.app -o src/MyApp -n MyApp
   dotnet sln TempeHS.CSharp.slnx add src/MyApp
   ```

3. Update the project paths in [`.vscode/tasks.json`](.vscode/tasks.json) and
   [`.vscode/launch.json`](.vscode/launch.json) (search for `HelloWorld`).
4. Delete the `src/HelloWorld.*` folders once you no longer need them
   (`dotnet sln TempeHS.CSharp.slnx remove src/HelloWorld.Console`).

---

## 8. Continuous integration

[`.github/workflows/build.yml`](.github/workflows/build.yml) runs on every push to `main`, on
pull requests, and on demand (**Actions ▸ Build ▸ Run workflow**). It:

1. restores, checks formatting (`dotnet format --verify-no-changes`), builds and runs tests;
2. publishes self-contained executables for `linux-x64`, `win-x64`, `osx-arm64` and `osx-x64`;
3. uploads each as an artifact (**Actions ▸ latest run ▸ Artifacts**) – handy for getting a Mac
   build to a Mac, or a Windows build to a Windows PC;
4. downloads the Windows artifact onto a **real Windows runner** and runs the console `.exe`.

---

## 9. Troubleshooting

| Problem                                                       | Fix                                                                                                                                                       |
| ------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| noVNC page shows "Failed to connect"                          | The desktop takes ~20 s to start after the container boots. Refresh. Still broken? Run `/usr/local/share/desktop-init.sh`.                                |
| App window never appears / `XOpenDisplay failed`              | Run `export DISPLAY=:1` in that terminal, or open a new VS Code terminal.                                                                                 |
| `wine: command not found` / `dotnet new avalonia.app` unknown | `setup.sh` did not finish. Run `bash .devcontainer/setup.sh` manually and check for errors.                                                               |
| Windows GUI app shows a blank white window under Wine         | Avalonia's GPU renderer doesn't work under Wine. Keep the Wine-detection snippet from [`Program.cs`](src/HelloWorld.Desktop/Program.cs) in your own apps.|
| `NETSDK1031: … self-contained … without specifying a RuntimeIdentifier` | Add `-r linux-x64` (or another runtime) to your `dotnet publish` command.                                                                       |
| `NETSDK1100: … Windows … is only supported on Windows`        | Building WinForms/WPF needs `EnableWindowsTargeting=true` – it is set in the container; in CI add `-p:EnableWindowsTargeting=true`.                        |
| C# Dev Kit asks you to sign in                                | Sign in with your GitHub account – C# Dev Kit is free for students and individuals. Or just use the C# extension (it is installed too).                    |
| IntelliSense shows errors everywhere                          | `Ctrl+Shift+P` ▸ *.NET: Restart Language Server*, or *Developer: Reload Window*. Make sure the solution restored (`dotnet restore`).                       |
| `dotnet format --verify-no-changes` fails in CI               | Run `dotnet format TempeHS.CSharp.slnx` locally, commit the result.                                                                                       |
| Windows SmartScreen blocks the `.exe`                         | Normal for unsigned apps. Click **More info ▸ Run anyway**. Code signing is outside the scope of this template.                                             |
| Codespace is slow                                             | Machine type must be ≥ 4 cores / 8 GB (set in `hostRequirements`). Change it under *Codespaces ▸ … ▸ Change machine type*.                               |

---

## 10. Further documentation

- [`docs/avalonia-basics.md`](docs/avalonia-basics.md) – how the Hello World desktop app is put
  together: XAML, code-behind, controls, layout, events.
- [`docs/new-project.md`](docs/new-project.md) – creating new console, Avalonia, class library,
  xUnit test, WinForms and WPF projects and adding them to the solution.
- [`docs/publishing.md`](docs/publishing.md) – runtime identifiers, self-contained vs
  framework-dependent, single-file, trimming, icons, version numbers, macOS `.app` bundles.
- External: [.NET documentation](https://learn.microsoft.com/dotnet/),
  [C# language reference](https://learn.microsoft.com/dotnet/csharp/),
  [Avalonia documentation](https://docs.avaloniaui.net/),
  [`dotnet` CLI reference](https://learn.microsoft.com/dotnet/core/tools/).
