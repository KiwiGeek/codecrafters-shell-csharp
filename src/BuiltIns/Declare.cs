namespace CodeCrafters.Shell.BuiltIns;

internal class Declare : IBuiltInCommand
{
    public string CommandName => "declare";

    public void Execute(Token[] tokens, CommandContext context)
    {
        // no op
    }

}