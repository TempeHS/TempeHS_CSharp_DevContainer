using System.Runtime.InteropServices;

// A minimal console "Hello, World!" that also reports which platform it
// is running on, so you can tell the Linux and Windows builds apart.
Console.WriteLine("Hello, World!");
Console.WriteLine($"Running on: {RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture})");
Console.WriteLine($"Runtime:    {RuntimeInformation.FrameworkDescription}");
