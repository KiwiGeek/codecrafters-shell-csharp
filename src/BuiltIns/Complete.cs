namespace CodeCrafters.Shell.BuiltIns;

internal class Complete : IBuiltInCommand
{
    public string CommandName => "complete";

    public void Execute(Token[] tokens)
    {
        Console.WriteLine(string.Join(" ", tokens.AsSpan()[..]));
    }

}