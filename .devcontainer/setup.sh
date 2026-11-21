#!/usr/bin/env bash
# Post-create setup for the C# Desktop devcontainer.
#
# The base image already ships the .NET 10 SDK. This script adds:
#   * the native X11 / fontconfig libraries Avalonia (via SkiaSharp) needs to
#     draw windows on the virtual desktop,
#   * Wine, so a Windows .exe published here can be smoke-tested in the
#     virtual desktop,
#   * the Avalonia project templates (`dotnet new avalonia.app` etc.),
#   * a warm NuGet cache for the sample solution.
set -euo pipefail

export DEBIAN_FRONTEND=noninteractive

echo "==> Installing native libraries for Avalonia, xterm and Wine"
sudo apt-get update
sudo apt-get install -y --no-install-recommends \
  libice6 libsm6 libfontconfig1 libfreetype6 \
  libx11-6 libx11-xcb1 libxcb1 libxrandr2 libxi6 libxcursor1 libxext6 libxrender1 \
  libgl1 libegl1 libgbm1 \
  fonts-dejavu-core fonts-liberation \
  xterm file \
  wine wine64
sudo apt-get clean
sudo rm -rf /var/lib/apt/lists/*

echo "==> Installing Avalonia project templates"
dotnet new install Avalonia.Templates >/dev/null

echo "==> Restoring NuGet packages for the sample solution"
dotnet restore TempeHS.CSharp.slnx

echo "==> Initialising 64-bit Wine prefix (used to smoke-test Windows builds)"
export WINEARCH=win64
export WINEDEBUG=-all
wineboot --init >/dev/null 2>&1 || true

echo
echo "==> Toolchain summary"
dotnet --version
dotnet new list avalonia 2>/dev/null | grep -i 'avalonia' | head -n 3 || true
wine --version
echo
echo "Setup complete."
echo "  * Press F5 to run the Hello World desktop app (it appears in the virtual desktop)."
echo "  * Open forwarded port 6080 (password: vscode) to see the virtual desktop."
