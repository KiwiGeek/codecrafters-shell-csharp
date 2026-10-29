using CodeCrafters.Shell.Classes;

namespace CodeCrafters.Shell.BuiltIns;

internal class Pwd : IBuiltInCommand
{
    public void Execute(Token[] args)
    {
        Console.WriteLine(Environment.CurrentDirectory);
    }

    public string CommandName => "pwd";
}