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
        TextWriter originalOutput = Console.Out;

        while (true)
        {
            Console.Write("$ ");
            string input = Console.ReadLine() ?? string.Empty;

            Token[] tokens = input.Tokenize();
            
            // Split the input into command and arguments
            Token command = tokens.First();
            bool handled = false;
            TextWriter output = Console.Out;
            
            // see if one of the tokens is a StandoutOutput redirect.
            if (tokens.Any(t => t.Value is ">" or "1>"))
            {
                int standardRedirectIndex = Array.IndexOf(tokens, tokens.First(d=> d.Value is ">" or "1>"));
                string outputFileName = tokens[standardRedirectIndex+ 1].Value;
                output = new StreamWriter(outputFileName, false);
                // remove the two tokens we just consumed
                tokens = [.. tokens[..standardRedirectIndex], .. tokens[(standardRedirectIndex+2)..]];
            }
            
            Console.SetOut(output);
            
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
                    startInfo.RedirectStandardOutput = true;
                    Process? process = Process.Start(startInfo);
                    process!.WaitForExit();
                    output.Write(process.StandardOutput.ReadToEnd());
                }
                else
                {
                    Console.WriteLine($"{command.Value}: command not found");
                }
            }
            if (output != originalOutput)
            {
                output.Flush();
                if (output is StreamWriter sw)
                {
                    sw.Close();
                }
                Console.SetOut(originalOutput);
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
}