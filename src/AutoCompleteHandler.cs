namespace CodeCrafters.Shell;

internal class AutoCompleteHandler(IBuiltInCommand[] builtInCommands) : IAutoCompleteHandler
{
    public char[] Separators { get; set; } = [' ', '.', '/'];
    
    public string[] GetSuggestions(string text, int index)
    {
        // if the text we are supplied doesn't have a space in it yet, we're trying to autocomplete an executable
        if (!text.Contains(' ')) return GetExecutableSuggestions(text, index);
        
        // extract the current word, via finding the last space in text, and taking to the end of the string
        string lastWord = text[(text.LastIndexOf(' ')+1)..];
        
        // see if we have registered completions for the first token
        Token[] tokens = text.Tokenize();
        string command = tokens.First();
        
        // get a reference to the completion builtin
        Complete completeBuiltIn = (Complete)Program.Builtins.Single(f => f.CommandName == "complete");
        if (completeBuiltIn.HasCompletion(command))
        {
            // for complete completions, we want to extract three arguments to pass to the completer script
            // arg[1] = the command
            // arg[2] = the word being completed, if anything typed so far
            // arg[3] = the previous word
            string completeArg1 = command;
            string completeArg2 = lastWord;
            string completeArg3 = text.Substring(completeArg1.Length, text.Length - completeArg1.Length - completeArg2.Length).Trim().Split(' ').Last();
            if (completeArg3.IsWhiteSpace()) completeArg3 = completeArg1;
            List<string> completeResults = completeBuiltIn.ExecuteCompletion(
                text, 
                text.Length, 
                completeArg1, 
                completeArg2, 
                completeArg3).ToList();
            return NothingBellAndLongestCommonPrefix(completeResults, lastWord);
        }
        
        
        return GetFilenamesFromCurrentDirectory(lastWord, text);
        
    }

    private string[] GetExecutableSuggestions(string text, int index)
    {
        
        List<string> suggestions =
        [
            .. from command in builtInCommands
            where command.CommandName.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)
            select $"{command.CommandName} "
        ];

        PathHelpers.AllExecutablesInPath(text)
            .ToList()
            .ForEach(d => suggestions.Add($"{d} "));

        return NothingBellAndLongestCommonPrefix(suggestions, text);
    }

    private string[] GetFilenamesFromCurrentDirectory(string lastWord, string text)
    {
        List<string> files = Directory
            .GetFiles(Directory.GetCurrentDirectory(), $"{lastWord}*")
            .Select(f => Path.GetFileName(f) + " ")
            .ToList();
        files.AddRange(Directory
            .GetDirectories(Directory.GetCurrentDirectory(), $"{lastWord}*")
            .Select(f => Path.GetFileName(f) + "/"));
        return NothingBellAndLongestCommonPrefix(files, lastWord);
    }
    
    
    private string[] NothingBellAndLongestCommonPrefix(List<string> suggestions, string text)
    {
        if (suggestions.Count == 0)
        {
            Console.Write("\a");
        }   
        else if (suggestions.Distinct().Count() > 1)
        {
            // determine if there is a common prefix, and if there is, find the longest common prefix.

            // find the shortest suggested completion length
            int shortestLength = suggestions.Min(s => s.Length);
            int longestCommonPrefix = 0;

            for (int i = text.Length; i < shortestLength; i++)
            {
                if (suggestions.All(f => f.StartsWith(suggestions.First()[..i], StringComparison.InvariantCultureIgnoreCase)))
                {
                    longestCommonPrefix = i;
                }
            }
            if (longestCommonPrefix > 0 && longestCommonPrefix != text.Length)
            {
                return [string.Concat(text,suggestions.First().Substring(text.Length,  longestCommonPrefix - text.Length)) ];
            }
        }
        
        return [.. suggestions.Distinct()];
    }
    
}