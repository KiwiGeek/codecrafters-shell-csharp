using System.Diagnostics;
using CodeCrafters.Shell.BuiltIns;
using CodeCrafters.Shell.Classes;
using CodeCrafters.Shell.Extensions;
using ReadLine;
using Type = CodeCrafters.Shell.BuiltIns.Type;
using ReadLineExt = ReadLine.ReadLine;

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
        TextWriter originalError = Console.Error;

        while (true)
        {

            ReadLineExt.Context.AutoCompletionHandler = new AutoCompleteHandler();
            
            string input = ReadLineExt.Read("$ ", string.Empty)!;
            Token[] tokens = input.Tokenize();
            if (tokens.Length == 0) continue;
            // Split the input into command and arguments
            Token command = tokens.First();
            bool handled = false;
            TextWriter output = Console.Out;
            TextWriter error = Console.Error;
            
            // see if one of the tokens is a StandoutOutput redirect.
            if (tokens.Any(t => t.Value is ">" or "1>" or ">>" or "1>>"))
            {
                int standardRedirectIndex = Array.IndexOf(tokens, tokens.First(d=> d.Value is ">" or "1>" or ">>" or "1>>"));
                string outputFileName = tokens[standardRedirectIndex+ 1].Value;
                bool append = tokens[standardRedirectIndex].Value.Contains(">>");
                output = new StreamWriter(outputFileName, append);
                // remove the two tokens we just consumed
                tokens = [.. tokens[..standardRedirectIndex], .. tokens[(standardRedirectIndex+2)..]];
            }
            
            if (tokens.Any(t => t.Value is "2>" or "2>>"))
            {
                int standardErrorRedirectIndex = Array.IndexOf(tokens, tokens.First(d=> d.Value is "2>" or "2>>"));
                string errorFileName = tokens[standardErrorRedirectIndex+ 1].Value;
                bool errorAppend = tokens[standardErrorRedirectIndex].Value.Contains(">>");
                error = new StreamWriter(errorFileName, errorAppend);
                // remove the two tokens we just consumed
                tokens = [.. tokens[..standardErrorRedirectIndex], .. tokens[(standardErrorRedirectIndex+2)..]];
            }
            
            Console.SetOut(output);
            Console.SetError(error);
            
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
                    startInfo.RedirectStandardError = true;
                    Process? process = Process.Start(startInfo);
                    process!.WaitForExit();
                    output.Write(process.StandardOutput.ReadToEnd());
                    error.Write(process.StandardError.ReadToEnd());
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

            if (error != originalError)
            {
                error.Flush();
                if (error is StreamWriter errorStream)
                {
                    errorStream.Close();
                }
                Console.SetError(originalError);
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    class AutoCompleteHandler : IAutoCompleteHandler
    {
        public string[] GetSuggestions(string text, int index)
        {
            List<string> suggestions = [];
            foreach (IBuiltInCommand command in Builtins)
            {
                if (command.CommandName.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
                {
                    suggestions.Add($"{command.CommandName} ");
                }
            }
            return suggestions.ToArray();
        }

        public char[] Separators { get; set; } = new char[] { ' ', '.', '/' };
    }
}