using CodeCrafters.Shell.Classes;
using CodeCrafters.Shell.Helpers;

namespace CodeCrafters.Shell.BuiltIns;

internal class Type : IBuiltInCommand
{
    public string CommandName => "type";

    public void Execute(Token[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: type <command>");
        }
        else if (Program.Builtins.Any(b => b.CommandName == args[0]))
        {
            Console.WriteLine($"{args[0].Value} is a shell builtin");
        }
        else
        {
           string? resolveFromPath = ExecutableHelpers.ResolveExecutableFromPath(args[0]);
           if (resolveFromPath != null)
           {
               Console.WriteLine($"{args[0]} is {resolveFromPath}");
           }
           else
           {
               Console.WriteLine($"{args[0]}: not found");   
           }
        }
    }
}