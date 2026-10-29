using System.Diagnostics;
using CodeCrafters.Shell.BuiltIns;
using CodeCrafters.Shell.Classes;
using CodeCrafters.Shell.Extensions;
using Type = CodeCrafters.Shell.BuiltIns.Type;

namespace CodeCrafters.Shell;

internal static class Program
{

    internal static readonly IBuiltInCommand[] Builtins = [
        new Exit(),
        new Echo(),
        new Type(),
        new Pwd(),
        new Cd()
    ];

    static void Main()
    {

        while (true)
        {
            Console.Write("$ ");
            string input = Console.ReadLine() ?? string.Empty;

            Token[] tokens = input.Tokenize();
            
            // Split the input into command and arguments
            Token command = tokens.First();
            bool handled = false;

            // try to process built in commands
            foreach (IBuiltInCommand builtin in Builtins)
            {
                if (builtin.CommandName.Equals(command.Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    builtin.Execute(tokens[1..]);
                    handled = true;
                    break;
                }
            }
            if (!handled)
            {
                string? resolveFromPath = Helpers.ExecutableHelpers.ResolveExecutableFromPath(command.Value);
                if (resolveFromPath != null)
                {
                    
                    // execute the executable with any provided arguments
                    ProcessStartInfo startInfo = new () { FileName = command.ToString() };
                    tokens[1..].ToList().ForEach(t => startInfo.ArgumentList.Add(t!));
                    Process? process = Process.Start(startInfo);
                    process!.WaitForExit();
                }
                else
                {
                    Console.WriteLine($"{command.Value}: command not found");
                }
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
}