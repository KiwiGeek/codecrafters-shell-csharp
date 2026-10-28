internal class Type : IBuiltInCommand
{
    public string CommandName => "type";

    public void Execute(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: type <command>");
            return;
        }
        if (Program.builtins.Any(b => b.CommandName == args[0]))
        {
            Console.WriteLine($"{args[0]} is a shell builtin");
        }
        else
        {
            Console.WriteLine($"{args[0]}: not found");
        }
    }
}