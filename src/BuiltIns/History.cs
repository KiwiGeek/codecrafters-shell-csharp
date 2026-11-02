namespace CodeCrafters.Shell.BuiltIns;

internal class History : IBuiltInCommand
{

    private List<string> _history = [];
    
    public void RegisterHistory(string command)
    {
        _history.Add(command);
    }
    
    public string CommandName => "history";
    public void Execute(Token[] tokens, CommandContext context)
    {
        List<string> potentialLines = [];
        
        foreach (var (item, index) in _history.Select((value, idx) => (value, idx+1)))
        {
            potentialLines.Add($"{index.ToString().PadLeft(5)}  {item}");
        }

        int maxLines = potentialLines.Count;
        
        // if we received a token, and it's a number, that's the number of maximum lines (clamped at 0 and maxLines)
        if (tokens.Any())
        {
            string firstToken = tokens.First();
            if (int.TryParse(firstToken, out var firstTokenInt))
            {
                maxLines = Math.Max(0, firstTokenInt - 1);
                maxLines = Math.Min(maxLines, firstTokenInt);
            }
        }
        
        // select the last {maxlines} from history.
        int rowsToSkip = potentialLines.Count - maxLines;

        foreach (string s in potentialLines.Skip(rowsToSkip-1))
        {
            context.StandardOutput.WriteLine(s);
        }
    }
}