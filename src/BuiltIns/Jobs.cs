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
        public char MostRecentMarker { get; private set; }
        private Process Process { get; init; }

        public void DecrementRecentMarker()
        {
            MostRecentMarker = MostRecentMarker switch { '+' => '-', _ => ' ' };
        }

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
            MostRecentMarker = '+';
            Process = process;
        }
    }

    internal List<JobInfo> _jobs = [];

    public void RegisterBackgroundJob(Process process, string command)
    {
        _backgroundSeqId++;
        _jobs.ForEach(j => j.DecrementRecentMarker());
        _jobs.Add(new JobInfo(_backgroundSeqId, process.Id, command, "Running", process));
        Console.WriteLine($"[{_backgroundSeqId}] {process.Id}");
    }
    
    
    public string CommandName => "jobs";

    public void Execute(Token[] tokens)
    {
        foreach (JobInfo job in _jobs)
        {
            job.CheckForDeadProcesses();
            Console.WriteLine($"[{job.JobId}]{job.MostRecentMarker}  {job.Status,-24}{job.Command}");
        }

        _jobs.RemoveAll(j => j.Status == "Done");
    }

}