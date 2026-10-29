namespace CodeCrafters.Shell.Helpers;

internal static class ExecutableHelpers
{
    internal static string? ResolveExecutableFromPath(string path)
    {
        if (OperatingSystem.IsWindows())
        {
            // for windows, if the path we were provided has an extension, we just do the same as other
            // operating systems. 
            if (path.Contains('.')) return PathHelpers.ResolveExecutable(path);
            
            // but if we weren't provided an extension, we want to try common windows executable extensions.
            // this is contained in the PATHEXT environment variable.
            string? extensions = Environment.GetEnvironmentVariable("PATHEXT");
            if (extensions != null)
            {
                foreach (string extension in extensions.Split(Path.PathSeparator))
                {
                    string? resolvedPath = PathHelpers.ResolveExecutable($"{path}{extension}");
                    if (resolvedPath != null) return resolvedPath;
                }
            }

            return null;
        }
        else
        {
            // for non-windows, we don't have to do anything funky. We just
            // interrogate the environment path for the name provided.
            return PathHelpers.ResolveExecutable(path);
        }
    }

    internal static bool IsExecutable(string path)
    {
        if (OperatingSystem.IsWindows())
        {
            return File.Exists(path);
        }
        UnixFileMode mode = File.GetUnixFileMode(path);
        return (mode & UnixFileMode.UserExecute) == UnixFileMode.UserExecute;
    }
}