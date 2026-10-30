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

    internal static string[] AllExecutablesInPath(string? startsWith)
    {
        startsWith ??= string.Empty;
        List<string> results = [];
        string? pathVariable = Environment.GetEnvironmentVariable("PATH");
        string? extensions = Environment.GetEnvironmentVariable("PATHEXT");
        StringComparison comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (pathVariable != null)
        {
            string[] paths = pathVariable.Split(Path.PathSeparator);
            foreach (string path in paths)
            {
                if (!Directory.Exists(path)) continue;
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    // see if the file begins with:
                    if (!Path.GetFileName(file).StartsWith(startsWith, comparison) && !string.IsNullOrEmpty(startsWith)) continue;
                    if (OperatingSystem.IsWindows() && extensions != null)
                    {
                        results.AddRange(
                            from extension 
                            in extensions.Split(Path.PathSeparator) 
                            where file.EndsWith(extension, comparison)
                            where ExecutableHelpers.IsExecutable(file) 
                            select Path.GetFileName(file));
                    }
                    else
                    {
                        // on Unix based systems, we want to check if the file is executable
                        if (ExecutableHelpers.IsExecutable(file)) results.Add(Path.GetFileName(file));
                    }
                }
            }
        }

        return results.ToArray();
    }
}