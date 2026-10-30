namespace CodeCrafters.Shell.BuiltIns;

internal class Echo : IBuiltInCommand
{
    public string CommandName => "echo";

    public void Execute(Token[] tokens)
    {
        Console.WriteLine(string.Join(" ", tokens.AsSpan()[..]));
    }

}