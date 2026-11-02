namespace CodeCrafters.Shell.BuiltIns;

internal class History : IBuiltInCommand
{
    public string CommandName => "history";
    public void Execute(Token[] tokens, CommandContext context)
    {
        //no op
    }
}