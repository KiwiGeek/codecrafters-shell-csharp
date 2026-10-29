using CodeCrafters.Shell.Classes;

namespace CodeCrafters.Shell.BuiltIns;

internal class Echo : IBuiltInCommand
{
    public string CommandName => "echo";

    public void Execute(Token[] args)
    {
        Console.WriteLine(string.Join(" ", args[0..]));
    }

}