using System.Text.Json.Serialization;

namespace CodeCrafters.Shell.BuiltIns;

public class Jobs : IBuiltInCommand
{
    private int _backgroundSeqId;

    internal class JobInfo
    {
        public string Status { get; private set; }
        public string Command { get; private set; }
        public int JobId { get; init; }
        public int ProcessId { get; init; }
        private Process Process { get; init; }

        public void CheckForDeadProcesses()
        {
            // we need to check if the process has quit
            if (Process.HasExited)
            {
                Status = "Done";
                Command = Command.Replace(" &", "");
            }
        }
        
        internal JobInfo(int jobId, int processId, string command, string status, Process process) 
        {
            JobId = jobId;
            ProcessId = processId;
            Command = command;
            Status = status;
            Process = process;
        }
    }

    internal List<JobInfo> _jobs = [];

    public void RegisterBackgroundJob(Process process, string command)
    {
        _backgroundSeqId++;
        _jobs.Add(new JobInfo(_backgroundSeqId, process.Id, command, "Running", process));
        Console.WriteLine($"[{_backgroundSeqId}] {process.Id}");
    }
    
    
    public string CommandName => "jobs";

    public void Execute(Token[] tokens)
    {
        int highestSeqId = _jobs.OrderByDescending(f => f.JobId).FirstOrDefault()?.JobId ?? 0;
        int secondHighestSeqId = _jobs.Count<2 ? 0 : _jobs.OrderByDescending(f => f.JobId).Skip(1).First().JobId;
        foreach (JobInfo job in _jobs)
        {
            job.CheckForDeadProcesses();
            char marker = ' ';
            if (job.JobId == highestSeqId) marker = '+';
            else if (job.JobId == secondHighestSeqId) marker = '-';
            Console.WriteLine($"[{job.JobId}]{marker}  {job.Status,-24}{job.Command}");
        }

        _jobs.RemoveAll(j => j.Status == "Done");
    }

    public void BackgroundReap()
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
                Console.WriteLine($"[{job.JobId}]{marker}  {job.Status,-24}{job.Command}");
            }
        }
        _jobs.RemoveAll(j => j.Status == "Done");
    }
}