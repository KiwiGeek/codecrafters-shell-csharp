namespace CodeCrafters.Shell;

internal static class Program
{

    internal static readonly IBuiltInCommand[] Builtins = [
        new Exit(), new Echo(), new Type(), new Pwd(), new Cd(), new Complete(), new Jobs(), new History()
    ];

    static async Task Main()
    {
        TextWriter originalOutput = Console.Out;
        TextWriter originalError = Console.Error;
        TextReader originalInput = Console.In;
        Jobs jobBuiltIn = Builtins.Single(f => f.CommandName == "jobs") as Jobs ??
                          throw new InvalidOperationException();
        History historyBuiltIn = Builtins.Single(f => f.CommandName == "history") as History ??
                                 throw new InvalidOperationException();
        PipelineExecutor executor = new ()
        {
            Builtins = Builtins,
            ConsoleOutput = originalOutput,
            ConsoleError = originalError,
            ConsoleInput = originalInput,
            JobsBuiltin = jobBuiltIn
        };
        
        ReadLineExt.Context.AutoCompletionMode = AutoCompletionModes.ListAll;
        ReadLineExt.Context.AutoCompletionHandler = new AutoCompleteHandler(Builtins);

        while (true)
        {
            
            try
            {
                jobBuiltIn.BackgroundReap(originalOutput);
                string input = ReadLineExt.Read("$ ", string.Empty)!;
                historyBuiltIn.RegisterHistory(input);
                Pipeline pipeline = input.Tokenize();
                await executor.ExecutePipeline(pipeline);
            }
            catch (Exception ex)
            {
                originalError.WriteLine(ex);
            }
           
        }
        // ReSharper disable once FunctionNeverReturns
    }
}