namespace CodeCrafters.Shell.BuiltIns;

internal class Complete : IBuiltInCommand
{
    public string CommandName => "complete";

    private Dictionary<string, string> _completions = [];

    public void Execute(Token[] tokens)
    {

        if (tokens.Length == 0)
        {
            Console.WriteLine("Nothing to do");
        }
        else
        {
            switch (tokens.First())
            {
                case "-C":
                    // the -C flag needs two tokens following.
                    if (tokens.Count() != 3)
                    {
                        Console.WriteLine("Wrong number of arguments");
                    }
                    else
                    {
                        string completionScriptToRegister =  tokens.Skip(1).First();
                        string completionCommandToRegister = tokens.Skip(2).First();
                        _completions[completionCommandToRegister] = completionScriptToRegister;
                    }
                    break;
                case "-p":
                    // the -p flag needs a second token which is the command the completion script is for.
                    if (tokens.Count() == 1)
                    {
                        Console.WriteLine("Need command name after -p");
                    }
                    else
                    {
                        string commandToPrint = tokens.Skip(1).First();
                        if (_completions.TryGetValue(commandToPrint, out string? completionScriptToPrint))
                        {
                            Console.WriteLine($"complete -C '{completionScriptToPrint}' {commandToPrint}");
                        }
                        else
                        {
                            Console.WriteLine($"complete: {commandToPrint}: no completion specification");
                        }
                    }

                    break;
                default:
                    Console.WriteLine("Unrecognized command");
                    break;
            }
        }
    }
    
    public bool HasCompletion(string command)
    {
        return _completions.ContainsKey(command);
    }

    public string[] ExecuteCompletion(string commandArg1,  string commandArg2, string commandArg3)
    {
        string completerScript = _completions[commandArg1];
        ProcessStartInfo startInfo = new () { FileName = completerScript };
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.ArgumentList.Add(commandArg1);
        startInfo.ArgumentList.Add(commandArg2);
        startInfo.ArgumentList.Add(commandArg3);
        Process? process = Process.Start(startInfo);
        process!.WaitForExit();
        string result = process.StandardOutput.ReadToEnd();
        if (result.Length == 0) return []; 
        result = result.Trim() + ' ';
        return [result];
        // string[] lines = result.Split(Environment.NewLine);
        // List<string> completions = [.. lines.Select(line => line + ' ')];
        // return [.. completions.Distinct().Concat(completions).ToArray()];
    }

}