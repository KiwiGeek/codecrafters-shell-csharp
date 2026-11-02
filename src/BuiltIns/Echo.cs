namespace CodeCrafters.Shell.BuiltIns;

internal class Echo : IBuiltInCommand
{
    public string CommandName => "echo";

    public void Execute(Token[] tokens, CommandContext context)
    {
        context.StandardOutput.WriteLine(string.Join(" ", tokens.AsSpan()[..]));
    }

}