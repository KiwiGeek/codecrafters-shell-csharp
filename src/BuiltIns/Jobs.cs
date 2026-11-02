namespace CodeCrafters.Shell.BuiltIns;

public class Jobs : IBuiltInCommand
{
    
    internal class JobInfo
    {
        public string Status { get; private set; }
        public string Command { get; private set; }
        public string RawInvocation { get; set; }
        public int JobId { get; }
        public int ProcessId { get; init; }
        private Process Process { get; }

        public void CheckForDeadProcesses()
        {
            // we need to check if the process has quit
            if (Process.HasExited)
            {
                Status = "Done";
                RawInvocation = RawInvocation.Replace(" &", "");
            }
        }
        
        internal JobInfo(int jobId, int processId, string command, string status, Process process, string rawInvocation) 
        {
            JobId = jobId;
            ProcessId = processId;
            Command = command;
            Status = status;
            Process = process;
            RawInvocation = rawInvocation;
        }
    }

    internal List<JobInfo> _jobs = [];

    internal void RegisterBackgroundJob(string command, Process process, Pipeline pipeline, CommandContext context)
    {
        // find the lowest unused jobId > 0.
        int newId = 1;
        bool found = false;
        while (!found)
        {
            if (_jobs.Any(j => j.JobId == newId))
            {
                newId++;
                continue;
            }
            found = true;
        }
        _jobs.Add(new JobInfo(newId, process.Id, command, "Running", process, pipeline.Invocation));
        context.StandardOutput.WriteLine($"[{newId}] {process.Id}");
    }
    
    
    public string CommandName => "jobs";

    public void Execute(Token[] tokens, CommandContext context)
    {
        int highestSeqId = _jobs.OrderByDescending(f => f.JobId).FirstOrDefault()?.JobId ?? 0;
        int secondHighestSeqId = _jobs.Count<2 ? 0 : _jobs.OrderByDescending(f => f.JobId).Skip(1).First().JobId;
        foreach (JobInfo job in _jobs)
        {
            job.CheckForDeadProcesses();
            char marker = ' ';
            if (job.JobId == highestSeqId) marker = '+';
            else if (job.JobId == secondHighestSeqId) marker = '-';
            WriteStatusLine(job, marker, context.StandardOutput);
        }

        _jobs.RemoveAll(j => j.Status == "Done");
    }

    private void WriteStatusLine(JobInfo job, char marker, TextWriter output)
    {
        output.WriteLine($"[{job.JobId}]{marker}  {job.Status,-24}{job.RawInvocation}");
    }

    internal void BackgroundReap(TextWriter output)
    {
        int highestSeqId = _jobs.OrderByDescending(f => f.JobId).FirstOrDefault()?.JobId ?? 0;
        int secondHighestSeqId = _jobs.Count<2 ? 0 : _jobs.OrderByDescending(f => f.JobId).Skip(1).First().JobId;
        foreach (JobInfo job in _jobs)
        {
            job.CheckForDeadProcesses();
            char marker = ' ';
            if (job.JobId == highestSeqId) marker = '+';
            else if (job.JobId == secondHighestSeqId) marker = '-';
            if (job.Status == "Done")
            {
                job.RawInvocation = job.RawInvocation.Replace(" &", "");
                WriteStatusLine(job, marker, output);
            }
        }
        _jobs.RemoveAll(j => j.Status == "Done");
    }
    
    
}