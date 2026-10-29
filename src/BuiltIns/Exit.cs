namespace CodeCrafters.Shell.BuiltIns;

internal class Exit : IBuiltInCommand
{
    public string CommandName => "exit";

    public void Execute(string[] args)
    {
        Environment.Exit(0);
    }
}