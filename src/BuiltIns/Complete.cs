namespace CodeCrafters.Shell.BuiltIns;

internal class Complete : IBuiltInCommand
{
    public string CommandName => "complete";

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
                case "-p":
                    // the -p flag needs a second token which is the command the completion script is for.
                    if (tokens.Count() == 1)
                    {
                        Console.WriteLine("Need command name after -p");
                    }
                    else
                    {
                        string command = tokens.Skip(1).First();
                        Console.WriteLine($"complete: {command}: no completion specification");
                    }

                    break;
                default:
                    Console.WriteLine("Unrecognized command");
                    break;
            }
        }
    }

}