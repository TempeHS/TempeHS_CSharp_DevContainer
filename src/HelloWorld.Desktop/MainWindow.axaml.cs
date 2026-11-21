using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HelloWorld.Desktop;

public partial class MainWindow : Window
{
    private int _clickCount;

    public MainWindow()
    {
        // Loads MainWindow.axaml and creates the fields for every x:Name'd control.
        InitializeComponent();

        PlatformText.Text =
            $"Running on {RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture})\n" +
            $"{RuntimeInformation.FrameworkDescription}";
    }

    private void OnClickMe(object? sender, RoutedEventArgs e)
    {
        _clickCount++;
        CounterText.Text = _clickCount == 1
            ? "You clicked the button once."
            : $"You clicked the button {_clickCount} times.";
    }
}
