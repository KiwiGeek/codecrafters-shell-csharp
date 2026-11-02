namespace CodeCrafters.Shell.BuiltIns;

internal class Exit : IBuiltInCommand
{
    public string CommandName => "exit";
    public void Execute(Token[] tokens, CommandContext context)
    {
        Environment.Exit(0);
    }
}