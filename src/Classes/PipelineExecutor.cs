using System.IO.Pipelines;

namespace CodeCrafters.Shell.Classes;

internal class PipelineExecutor
{
    internal required IBuiltInCommand[] Builtins { get; init; }
    internal required TextWriter ConsoleOutput { get; init; }
    internal required TextWriter ConsoleError { get; init; }
    internal required TextReader ConsoleInput { get; init; }
    internal required Jobs JobsBuiltin { get; init; }
    internal required Declare DeclareBuiltin { get; init; }
    
    public async Task ExecutePipeline(Pipeline pipeline)
    {
        List<IDisposable> resources = [];

        try
        {
            int stageCount = pipeline.Count;
            
            Pipe[] pipes = Enumerable.Range(0, stageCount - 1).Select(_ => new Pipe()).ToArray();
            StreamReader[] pipeReaders = pipes.Select(p => new StreamReader(p.Reader.AsStream())).ToArray();
            StreamWriter[] pipeWriters = pipes.Select(p => new StreamWriter(p.Writer.AsStream())
                { AutoFlush = true }).ToArray();

            resources.AddRange(pipeReaders);
            resources.AddRange(pipeWriters);

            CommandContext[] contexts = new CommandContext[stageCount];

            for (int i = 0; i < stageCount; i++)
            {
                PipelineStage stage = pipeline[i];
                TextReader input = i == 0 ? ConsoleInput : pipeReaders[i - 1];
                TextWriter output = i == stageCount - 1 ? ConsoleOutput : pipeWriters[i];
                TextWriter error = ConsoleError;
                
                if (stage.StandardOutputRedirect != null)
                {
                    output = new StreamWriter(stage.StandardOutputRedirect, stage.StandardOutputAppend);
                    resources.Add(output);
                }

                if (stage.StandardErrorRedirect != null)
                {
                    error = new StreamWriter(stage.StandardErrorRedirect, stage.StandardErrorAppend);
                    resources.Add(error);
                }

                contexts[i] = new CommandContext(input, output, error);
            }

            List<Task> runningTasks = [];

            for (int i = 0; i < pipeline.Count; i++)
            {
                PipelineStage stage = pipeline[i];
                CommandContext context = contexts[i];

                IBuiltInCommand? builtin = Builtins
                    .FirstOrDefault(b => b.CommandName.Equals(stage.Command, StringComparison.InvariantCultureIgnoreCase));

                if (builtin is not null)
                {
                    bool isLastStage = i == pipeline.Count - 1;
                    runningTasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            List<Token> args = [];
                            builtin.Execute(
                                stage.Arguments.Select(argument => 
                                    new Token(DeclareBuiltin.TryExpand(argument))).ToArray(), context);
                            context.StandardOutput.Flush();
                        }
                        finally
                        {
                            if (!isLastStage) context.StandardOutput.Dispose();
                        }
                    }));
                }
                else
                {
                    runningTasks.Add(ExecuteExternalStage(
                        stage,
                        context,
                        isFirstStage: i == 0,
                        isLastStage: i == pipeline.Count - 1,
                        pipeline));
                }
            }

            await Task.WhenAll(runningTasks);
        }
        finally
        {
            foreach (IDisposable resource in resources) resource.Dispose();
        }
    }
    
    private async Task ExecuteExternalStage(PipelineStage stage, CommandContext context, bool isFirstStage, 
                                            bool isLastStage, Pipeline pipeline)
    {
        string? executable = ExecutableHelpers.ResolveExecutableFromPath(stage.Command);

        if (executable is null)
        {
            await context.StandardError.WriteLineAsync($"{stage.Command}: command not found");
            if (!isLastStage) await context.StandardOutput.DisposeAsync();
            return;
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = stage.Command.ToString(),
            UseShellExecute = false,
            RedirectStandardInput = !isFirstStage,
            RedirectStandardOutput = !pipeline.BackgroundJob,
            RedirectStandardError = !pipeline.BackgroundJob
        };

        foreach (Token argument in stage.Arguments)
        {
            Token? value = DeclareBuiltin.TryExpand(argument);
            if (value != null) startInfo.ArgumentList.Add(value);
        }
    
        Process process = new() { StartInfo = startInfo };
        process.Start();

        if (pipeline.BackgroundJob)
        {
            JobsBuiltin.RegisterBackgroundJob(stage.Command, process, pipeline, context);
            return;
        }
        
        try
        {
            Task stdinTask = Task.CompletedTask;
            if (!isFirstStage) stdinTask = ForwardInputAsync(context, process);
            Task stdoutTask = ForwardOutputAsync(process, context, isLastStage);
            Task stderrTask = ForwardErrorAsync(process, context);

            await Task.WhenAll(
                stdinTask,
                stdoutTask,
                stderrTask,
                process.WaitForExitAsync());
        }
        finally
        {
            process.Dispose();
        }
        
        static async Task ForwardInputAsync(CommandContext context, Process process)
        {
            try
            {
                char[] buffer = new char[8192];
                int count;

                while ((count = await context.StandardInput.ReadAsync(buffer)) > 0)
                {
                    await process.StandardInput.WriteAsync(buffer.AsMemory(0, count));
                    await process.StandardInput.FlushAsync();
                }
            }
            finally
            {
                process.StandardInput.Close();
            }
        }

        static async Task ForwardOutputAsync(Process process, CommandContext context, bool isLastStage)
        {
            try
            {
                char[] buffer = new char[8192];
                int count;

                while ((count = await process.StandardOutput.ReadAsync(buffer)) > 0)
                {
                    await context.StandardOutput.WriteAsync(buffer.AsMemory(0, count));
                    await context.StandardOutput.FlushAsync();
                }
            }
            finally
            {
                if (!isLastStage) await context.StandardOutput.DisposeAsync();
            }
        }

        static async Task ForwardErrorAsync(Process process, CommandContext context)
        {
            char[] buffer = new char[8192];
            int count;

            while ((count = await process.StandardError.ReadAsync(buffer)) > 0)
            {
                await context.StandardError.WriteAsync(buffer.AsMemory(0, count));
                await context.StandardError.FlushAsync();
            }
        }
    }
}
