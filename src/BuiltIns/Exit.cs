namespace CodeCrafters.Shell.BuiltIns;

internal class Exit : IBuiltInCommand
{
    public string CommandName => "exit";
    public void Execute(Token[] tokens, CommandContext context)
    {
        History history = Program.Builtins.Single(f => f.CommandName == "history") as History ??
                          throw new InvalidOperationException();
        history.TryAppendHistoryFromEnvironment();
        Environment.Exit(0);
    }
}