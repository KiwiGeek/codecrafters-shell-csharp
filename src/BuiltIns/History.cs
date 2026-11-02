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
        foreach (var (item, index) in _history.Select((value, idx) => (value, idx+1)))
        {
            context.StandardOutput.WriteLine($"{index.ToString().PadLeft(5)}  {item}");
        }
    }
}