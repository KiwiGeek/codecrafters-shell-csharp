namespace CodeCrafters.Shell.BuiltIns;

public class Jobs : IBuiltInCommand
{
    private int _backgroundSeqId;

    internal class JobInfo
    {
        public string Status { get; set; }
        public string Command { get; init; }
        public int JobId { get; init; }
        public int ProcessId { get; init; }


        internal JobInfo(int jobId, int processId, string command, string status) 
        {
            JobId = jobId;
            ProcessId = processId;
            Command = command;
            Status = status;
        }
    }

    internal List<JobInfo> _jobs = [];

    public void RegisterBackgroundJob(int processId, string command)
    {
        _backgroundSeqId++;
        _jobs.Add(new JobInfo(_backgroundSeqId, processId, command, "Running"));
        Console.WriteLine($"[{_backgroundSeqId}] {processId}");
    }
    
    
    public string CommandName => "jobs";

    public void Execute(Token[] tokens)
    {
        foreach (JobInfo job in _jobs)
        {
            Console.WriteLine($"[{job.JobId}]+  {job.Status,-24}{job.Command}");
        }
    }

}