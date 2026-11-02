namespace CodeCrafters.Shell.BuiltIns;

internal class History : IBuiltInCommand
{

    private List<string> _history = [];
    private List<string> _appendableHistory = [];
    
    public void RegisterHistory(string command)
    {
        _history.Add(command);
        _appendableHistory.Add(command);
    }
    
    public string CommandName => "history";
    public void Execute(Token[] tokens, CommandContext context)
    {

        if (TryExecuteHistoryRead(tokens, context)) return;
        if (TryExecuteHistoryWrite(tokens, context)) return;
        if (TryExecuteHistoryAppend(tokens, context)) return;
        
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

    private bool TryExecuteHistoryRead(Token[] tokens, CommandContext context)
    {
        if (tokens.Length > 1 && tokens.First().Value.Equals("-r"))
        {
            string fileName =  tokens.Skip(1).First().Value;
            // see if the second token represents a real file.
            if (File.Exists(fileName))
            {
                foreach (string s in File.ReadAllLines(fileName))
                {
                    _history.Add(s);
                }

                return true;
            }
            context.StandardError.WriteLine($"{fileName} not found.");
        }

        return false;
    }

    private bool TryExecuteHistoryWrite(Token[] tokens, CommandContext context)
    {
        if (tokens.Length < 2 || tokens[0].Value != "-w") return false;
        string fileName =  tokens.Skip(1).First().Value;
        File.WriteAllLines(fileName, _history);
        return true;
    }
    
    private bool TryExecuteHistoryAppend(Token[] tokens, CommandContext context)
    {
        if (tokens.Length < 2 || tokens[0].Value != "-a") return false;
        string fileName =  tokens.Skip(1).First().Value;
        File.AppendAllLines(fileName, _appendableHistory);
        _appendableHistory.Clear();
        return true;
    }

    public void TryLoadHistoryFromEnvironment()
    {
        string? fileName = Environment.GetEnvironmentVariable("HISTFILE");
        if (fileName == null || !File.Exists(fileName)) return;
        foreach (string s in File.ReadAllLines(fileName)) _history.Add(s);
    }

    public void TryAppendHistoryFromEnvironment()
    {
        string? fileName = Environment.GetEnvironmentVariable("HISTFILE");
        if (fileName == null) return;
        File.AppendAllLines(fileName, _appendableHistory);
    }
}