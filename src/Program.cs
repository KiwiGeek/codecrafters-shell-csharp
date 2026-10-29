using CodeCrafters.Shell.BuiltIns;
using Type = CodeCrafters.Shell.BuiltIns.Type;

namespace CodeCrafters.Shell;

internal static class Program
{

    internal static readonly IBuiltInCommand[] Builtins = [
        new Exit(),
        new Echo(),
        new Type()
    ];

    static void Main()
    {

        while (true)
        {
            Console.Write("$ ");
            string input = Console.ReadLine() ?? string.Empty;
            
            // Split the input into command and arguments
            string[] args = input.Split(' ');
            string command = args[0];
            bool handled = false;

            // try to process built in commands
            foreach (IBuiltInCommand builtin in Builtins)
            {
                if (builtin.CommandName.Equals(command, StringComparison.InvariantCultureIgnoreCase))
                {
                    builtin.Execute(args[1..]);
                    handled = true;
                    break;
                }
            }
            if (!handled)
            {
                Console.WriteLine($"{input}: command not found");
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
}