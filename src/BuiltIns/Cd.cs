using CodeCrafters.Shell.Classes;

namespace CodeCrafters.Shell.BuiltIns;

internal class Cd : IBuiltInCommand
{
    public void Execute(Token[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: cd <path>");
            return;
        }
        if (args.First().ToString() == "~") args[0] = new Token(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        if (Directory.Exists(args.First()))
        {
            Environment.CurrentDirectory = args.First()!;
        }
        else
        {
            Console.WriteLine($"cd: {args.First().Value}: No such file or directory");
        }
    }

    public string CommandName => "cd";
}