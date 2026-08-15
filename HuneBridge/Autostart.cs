using Microsoft.Win32;

namespace HuneBridge;

internal static class Autostart
{
    private const string ValueName = "HuneBridge";
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string MutexName = @"Local\HuneBridge.SingleInstance";
    private static Mutex? _mutex;

    public static bool TryBecomeSingleInstance()
    {
        _mutex = new Mutex(true, MutexName, out var created);
        return created;
    }

    public static void Register()
    {
        if (!OperatingSystem.IsWindows())
            return;

        if (string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase))
            return;

        var exe = Environment.ProcessPath;
        if (string.IsNullOrWhiteSpace(exe)
            || !exe.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
            || string.Equals(Path.GetFileName(exe), "dotnet.exe", StringComparison.OrdinalIgnoreCase))
            return;

        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true)
                ?? Registry.CurrentUser.CreateSubKey(RunKey, true);
            if (key == null)
                return;

            var command = $"\"{exe}\"";
            var current = key.GetValue(ValueName) as string;
            if (!string.Equals(current, command, StringComparison.OrdinalIgnoreCase))
                key.SetValue(ValueName, command);
        }
        catch
        {
        }
    }
}
