namespace CodeCrafters.Shell.BuiltIns;

internal class Cd : IBuiltInCommand
{
    public void Execute(string[] args)
    {
        if (args[0] == "~") args[0] = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (Directory.Exists(args[0]))
        {
            Environment.CurrentDirectory = args[0];
        }
        else
        {
            Console.WriteLine($"cd: {args[0]}: No such file or directory");
        }
    }

    public string CommandName => "cd";
}