# Publishing – producing executables for Windows, Linux and macOS

`dotnet build` creates a *framework-dependent* `.dll` for development. `dotnet publish` creates
the files you give to other people. Because .NET is cross-platform, a Linux container can publish
for **all** supported operating systems – no Windows PC or Mac is needed to build.

## Runtime identifiers (RIDs)

| RID           | Target                                      |
| ------------- | ------------------------------------------- |
| `win-x64`     | Windows 10/11, 64-bit Intel/AMD (most PCs)  |
| `win-arm64`   | Windows on ARM (Surface Pro X, Copilot+ PCs) |
| `linux-x64`   | Linux, 64-bit Intel/AMD (incl. this container) |
| `linux-arm64` | Linux on ARM (Raspberry Pi 4/5 64-bit OS)   |
| `osx-arm64`   | macOS on Apple Silicon (M1 and later)       |
| `osx-x64`     | macOS on Intel                              |

Full list: [.NET RID catalog](https://learn.microsoft.com/dotnet/core/rid-catalog).

## The default in this template: one self-contained file

```bash
dotnet publish TempeHS.CSharp.slnx -c Release -r win-x64
#                                              ^^^^^^^^^ pick a RID from the table
```

Output: `publish/win-x64/HelloWorld.Desktop.exe` and `publish/win-x64/HelloWorld.Console.exe`.

This works because the `.csproj` files contain:

```xml
<PropertyGroup Condition="'$(RuntimeIdentifier)' != ''">
  <SelfContained>true</SelfContained>                      <!-- bundle the .NET runtime -->
  <PublishSingleFile>true</PublishSingleFile>              <!-- everything in one .exe -->
  <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
  <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>  <!-- roughly halves the size -->
  <DebugType>none</DebugType>                              <!-- no .pdb next to the .exe -->
</PropertyGroup>
```

and [`Directory.Build.props`](../Directory.Build.props) redirects the output of every project to
`publish/<RID>/`. Publishing a single project works the same way:

```bash
dotnet publish src/HelloWorld.Desktop -c Release -r linux-x64
```

**Pros:** runs on a clean machine with nothing installed; one file to hand in / share.
**Cons:** 40–50 MB per app (the runtime is inside); the first start takes a moment while it
unpacks to a temp folder.

## Smaller: framework-dependent

If the target machine already has the .NET 10 runtime installed (or you install it from
<https://dotnet.microsoft.com/download>), publish without bundling it:

```bash
dotnet publish src/HelloWorld.Console -c Release -r win-x64 --self-contained false -p:PublishSingleFile=false -o publish/win-x64-fd
```

Output is a few hundred KB (`HelloWorld.Console.exe` + `.dll` + `.runtimeconfig.json`).

## Even smaller: trimming (advanced)

```bash
dotnet publish src/HelloWorld.Console -c Release -r linux-x64 -p:PublishTrimmed=true
```

Removes unused parts of the runtime (console app: ≈ 13 MB). Trimming can break code that uses
reflection; Avalonia apps mostly trim fine but **test thoroughly**. Native AOT
(`-p:PublishAot=true`) goes further but requires the target OS's native toolchain, so it cannot
cross-compile for Windows from this container.

## Application icon, name and version

In the `.csproj`:

```xml
<PropertyGroup>
  <AssemblyName>MyApp</AssemblyName>                 <!-- file name: MyApp.exe -->
  <ApplicationIcon>Assets/app.ico</ApplicationIcon>  <!-- Windows .exe icon (.ico file) -->
  <Version>1.2.0</Version>                           <!-- shown in file properties on Windows -->
  <Authors>Your Name</Authors>
  <Product>My App</Product>
</PropertyGroup>
```

For the *window* icon in an Avalonia app, add `Icon="/Assets/app.ico"` to the `<Window>` element
and include the file as an `AvaloniaResource`:

```xml
<ItemGroup>
  <AvaloniaResource Include="Assets/**" />
</ItemGroup>
```

## Testing each build

| Target  | Inside the container                                   | Elsewhere                                                             |
| ------- | ------------------------------------------------------ | --------------------------------------------------------------------- |
| Linux   | `./publish/linux-x64/HelloWorld.Desktop` (virtual desktop) | Any x64 Linux with X11/Wayland                                    |
| Windows | `wine publish/win-x64/HelloWorld.Desktop.exe` (smoke test) | Download the `.exe` and run it; CI also runs the console `.exe` on Windows |
| macOS   | –                                                      | Download `publish/osx-arm64/HelloWorld.Desktop`, then `chmod +x` and run it |

### macOS notes

- A bare executable works from Terminal. For a double-clickable **`.app` bundle** you need an
  `Info.plist` and the folder structure `MyApp.app/Contents/MacOS/MyApp` – see the
  [Avalonia macOS deployment guide](https://docs.avaloniaui.net/docs/deployment/macOS).
- Gatekeeper will warn about an unsigned app. Right-click ▸ **Open** the first time, or remove the
  quarantine flag: `xattr -dr com.apple.quarantine MyApp.app`. Signing/notarising needs an Apple
  Developer account.

### Windows notes

- SmartScreen shows *"Windows protected your PC"* for unsigned apps – **More info ▸ Run anyway**.
- Console apps (`OutputType=Exe`) open a terminal window; GUI apps (`OutputType=WinExe`) do not.

## Getting the files out of the Codespace

- Right-click a file under `publish/` in the VS Code Explorer ▸ **Download**.
- Or push to `main` and download the **artifacts** from the *Actions* tab – the
  [CI workflow](../.github/workflows/build.yml) publishes Linux, Windows and macOS builds on
  every push.
- Or create a GitHub Release and attach the files (`gh release create v1.0 publish/win-x64/*.exe`).

## Cleaning up

```bash
rm -rf publish                     # remove published output
dotnet clean TempeHS.CSharp.slnx   # remove bin/obj build output
```
