namespace CodeCrafters.Shell;

internal class AutoCompleteHandler(IBuiltInCommand[] builtInCommands) : IAutoCompleteHandler
{
    public char[] Separators { get; set; } = [' ', '.', '/'];
    
    public string[] GetSuggestions(string text, int index)
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

        if (suggestions.Count == 0)
        {
            Console.WriteLine("\a");
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