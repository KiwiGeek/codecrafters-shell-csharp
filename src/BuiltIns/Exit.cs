using CodeCrafters.Shell.Classes;

namespace CodeCrafters.Shell.BuiltIns;

internal class Exit : IBuiltInCommand
{
    public string CommandName => "exit";

    public void Execute(Token[] args)
    {
        Environment.Exit(0);
    }
}