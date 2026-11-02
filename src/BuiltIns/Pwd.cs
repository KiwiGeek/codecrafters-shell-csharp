namespace CodeCrafters.Shell.BuiltIns;

internal class Pwd : IBuiltInCommand
{
    public string CommandName => "pwd";

    public void Execute(Token[] tokens, CommandContext context)
    {
        context.StandardOutput.WriteLine(Environment.CurrentDirectory);
    }
}