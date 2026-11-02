namespace CodeCrafters.Shell.Classes;

public class PipelineStage
{
    
    public Token Command { get; init; } 
    public string? StandardOutputRedirect { get; init; } 
    public string? StandardErrorRedirect { get; init; }
    public bool StandardOutputAppend { get; init; }
    public bool StandardErrorAppend { get; init; }
    public Token[] Arguments { get; init; }
    
    public PipelineStage(Token[] invocation)
    {
        // first, we tokenize that invocation to get the command and the arguments
        Token[] tokens = invocation;
        
        // assuming there was at least one token, the first is the command
        if (tokens.Length == 0) throw new ArgumentException( "Pipeline stage cannot be empty.", nameof(invocation));
        Command = tokens.First();
        
        // find any redirects, and store them
        if (tokens.Any(t => t.Value is ">" or "1>" or ">>" or "1>>"))
        {
            int standardRedirectIndex = Array.IndexOf(tokens, tokens.First(d=> d.Value is ">" or "1>" or ">>" or "1>>"));
            StandardOutputRedirect = tokens[standardRedirectIndex+ 1].Value;
            StandardOutputAppend = tokens[standardRedirectIndex].Value.Contains(">>");
            tokens = [.. tokens[..standardRedirectIndex], .. tokens[(standardRedirectIndex+2)..]];
        }
        if (tokens.Any(t => t.Value is "2>" or "2>>"))
        {
            int standardErrorRedirectIndex = Array.IndexOf(tokens, tokens.First(d=> d.Value is "2>" or "2>>"));
            StandardErrorRedirect = tokens[standardErrorRedirectIndex+ 1].Value;
            StandardErrorAppend = tokens[standardErrorRedirectIndex].Value.Contains(">>");
            tokens = [.. tokens[..standardErrorRedirectIndex], .. tokens[(standardErrorRedirectIndex+2)..]];
        }
        
        Arguments = (tokens.Length > 1) ? tokens[1..] : [];
    }
}