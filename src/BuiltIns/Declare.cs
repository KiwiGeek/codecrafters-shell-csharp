namespace CodeCrafters.Shell.BuiltIns;

internal class Declare : IBuiltInCommand
{
    private Dictionary<string, string> _declared = [];
    
    public string CommandName => "declare";

    public void Execute(Token[] tokens, CommandContext context)
    {
        // no op
        if (TryExecutePrint(tokens, context)) return;
        context.StandardError.WriteLine("Unknown command");
    }

    private bool TryExecutePrint(Token[] tokens, CommandContext context)
    {
        if (tokens.Length < 2) return false;
        if (!tokens.First().Value.Equals("-p")) return false;
        string variable = tokens.Skip(1).First().Value;
        context.StandardError.WriteLine($"declare: {variable}: not found");
        return true;
    }
}