namespace CodeCrafters.Shell.BuiltIns;

internal class Type : IBuiltInCommand
{
    public string CommandName => "type";

    public void Execute(Token[] tokens, CommandContext context)
    {
        if (tokens.Length < 1)
        {
            context.StandardOutput.WriteLine("Usage: type <command>");
        }
        else if (Program.Builtins.Any(b => b.CommandName == tokens.First()))
        {
            context.StandardOutput.WriteLine($"{tokens.First()} is a shell builtin");
        }
        else
        {
            string? resolveFromPath = ExecutableHelpers.ResolveExecutableFromPath(tokens.First());
            context.StandardOutput.WriteLine(resolveFromPath != null
                ? $"{tokens.First()} is {resolveFromPath}"
                : $"{tokens.First()}: not found");
        }
    }
}