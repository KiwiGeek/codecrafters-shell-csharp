namespace CodeCrafters.Shell.ReadLine;

public class ReadContext
{
    public AutoCompletionModes AutoCompletionMode { get; set; } = AutoCompletionModes.RotateThrough;
    public List<string> History { get; } = [];
    [PublicAPI] public bool HistoryEnabled { get; set; }
    public bool InsertionMode { get; set; }
    [PublicAPI] public IConsole Console { get; set; } = new Console2();
    public IAutoCompleteHandler? AutoCompletionHandler { get; set; }
}