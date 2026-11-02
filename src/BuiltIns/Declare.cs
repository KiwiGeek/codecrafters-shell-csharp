namespace CodeCrafters.Shell.BuiltIns;

internal class Declare : IBuiltInCommand
{
    private Dictionary<string, string> _declared = [];
    
    public string CommandName => "declare";

    public void Execute(Token[] tokens, CommandContext context)
    {
        if (TryExecutePrint(tokens, context)) return;
        if (TryStoreVariable(tokens, context)) return;
        context.StandardError.WriteLine("Unknown command");
    }

    private bool TryExecutePrint(Token[] tokens, CommandContext context)
    {
        if (tokens.Length < 2) return false;
        if (!tokens.First().Value.Equals("-p")) return false;
        string variable = tokens.Skip(1).First().Value;
        if (_declared.TryGetValue(variable, out var value))
        {
            context.StandardOutput.WriteLine($"declare -- {variable}=\"{value}\"");
        }
        else
        {
            context.StandardError.WriteLine($"declare: {variable}: not found");
        }
        return true;
    }

    private bool TryStoreVariable(Token[] tokens, CommandContext context)
    {
        if (tokens.Length < 1) return false;
        string command = tokens.First().Value;
        if (!command.Contains('=')) return false;
        string variableName = command[..command.IndexOf('=')];
        string value = command[(command.IndexOf('=') + 1)..];
        _declared.Remove(variableName);
        _declared.TryAdd(variableName, value);
        return true;
    }
}