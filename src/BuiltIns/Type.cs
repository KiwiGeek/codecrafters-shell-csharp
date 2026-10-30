namespace CodeCrafters.Shell.BuiltIns;

internal class Type : IBuiltInCommand
{
    public string CommandName => "type";

    public void Execute(Token[] tokens)
    {
        if (tokens.Length < 1)
        {
            Console.WriteLine("Usage: type <command>");
        }
        else if (Program.Builtins.Any(b => b.CommandName == tokens.First()))
        {
            Console.WriteLine($"{tokens.First()} is a shell builtin");
        }
        else
        {
            string? resolveFromPath = ExecutableHelpers.ResolveExecutableFromPath(tokens.First());
            Console.WriteLine(resolveFromPath != null
                ? $"{tokens.First()} is {resolveFromPath}"
                : $"{tokens.First()}: not found");
        }
    }
}