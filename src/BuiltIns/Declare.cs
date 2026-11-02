using System.Text.RegularExpressions;

namespace CodeCrafters.Shell.BuiltIns;

internal partial class Declare : IBuiltInCommand
{
    private Dictionary<string, string> _declared = [];
    
    public string CommandName => "declare";

    public void Execute(Token[] tokens, CommandContext context)
    {
        if (TryExecutePrint(tokens, context)) return;
        if (TryStoreVariable(tokens, context)) return;
        context.StandardError.WriteLine("Unknown command");
    }

    public Token TryExpand(Token token)
    {
        string variable = token.Value;
        foreach (KeyValuePair<string, string> declare in _declared)
        {
            variable = variable.Replace($"${declare.Key}", declare.Value);
        }

        return new Token(variable);
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
        if (!VariableNameRegex().IsMatch(variableName))
        {
            context.StandardError.WriteLine($"declare: `{command}': not a valid identifier");
        }
        string value = command[(command.IndexOf('=') + 1)..];
        _declared.Remove(variableName);
        _declared.TryAdd(variableName, value);
        return true;
    }

    [GeneratedRegex(@"^[_A-Za-z][_A-Za-z0-9]*$")]
    private static partial Regex VariableNameRegex();
}