namespace CodeCrafters.Shell.BuiltIns;

internal class Pwd : IBuiltInCommand
{
    public string CommandName => "pwd";

    public void Execute(Token[] tokens)
    {
        Console.WriteLine(Environment.CurrentDirectory);
    }
}