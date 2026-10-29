namespace CodeCrafters.Shell.BuiltIns;

internal class Pwd : IBuiltInCommand
{
    public void Execute(string[] args)
    {
        Console.WriteLine(Environment.CurrentDirectory);
    }

    public string CommandName => "pwd";
}