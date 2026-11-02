namespace CodeCrafters.Shell.BuiltIns;

internal class Cd : IBuiltInCommand
{
    public void Execute(Token[] tokens, CommandContext context)
    {
        if (tokens.Length < 1)
        {
            context.StandardOutput.WriteLine("Usage: cd <path>");
            return;
        }
        if (tokens.First().ToString() == "~") tokens[0] = new Token(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        if (Directory.Exists(tokens.First()))
        {
            Environment.CurrentDirectory = tokens.First();
        }
        else
        {
            context.StandardOutput.WriteLine($"cd: {tokens.First().Value}: No such file or directory");
        }
    }

    public string CommandName => "cd";
}