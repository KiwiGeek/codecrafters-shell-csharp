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

        if (suggestions.Count == 0) Console.WriteLine("\a");
        return [.. suggestions];
    }

}