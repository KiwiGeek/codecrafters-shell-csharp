namespace CodeCrafters.Shell.Helpers;

internal static class PathHelpers
{
    
    
    private static string? ResolveFile(string commandName, bool requireExecutable)
    {
        // case-insensitive operating system?
        StringComparison comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        
        string? pathValue = Environment.GetEnvironmentVariable("PATH");
        if (pathValue == null) return null;
        string[] paths = pathValue.Split(Path.PathSeparator);
        foreach (string path in paths)
        {
            if (!Directory.Exists(path)) continue;
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                if (Path.GetFileName(file).Equals(commandName, comparison))
                {
                    if (!requireExecutable) return file;
                    if (ExecutableHelpers.IsExecutable(file)) return file;
                }
            }
        }
        return null;
    }

    internal static string? ResolveFile(string commandName)
    {
        return ResolveFile(commandName, false);
    }
    
    internal static string? ResolveExecutable(string commandName)
    {
        return ResolveFile(commandName, true);
    }
}