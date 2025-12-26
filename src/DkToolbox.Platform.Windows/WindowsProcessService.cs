using System.Diagnostics;
using DkToolbox.Core;
using DkToolbox.Core.Abstractions;
using DkToolbox.Core.Models;

namespace DkToolbox.Platform.Windows;

public sealed class WindowsProcessService : IProcessService
{
    /// <inheritdoc />
    public IReadOnlyList<ProcessInfo> List(ProcessQuery query)
    {
        Process[] processes = Process.GetProcesses();
        
        IEnumerable<ProcessInfo> processInfos = processes.Select(process =>
        {
            long workingSet = 0;
            try
            {
                workingSet = process.WorkingSet64;
            }
            catch
            {
                // TODO: Log
            }
                
            return new ProcessInfo(process.Id, process.ProcessName, workingSet);
        });

        return ProcessListHelper.ApplyQuery(processInfos, query);
    }
    
    /// <inheritdoc />
    public KillResult Kill(int pid, KillOptions options)
    {
        try
        {
            var process = Process.GetProcessById(pid);
            process.Kill(entireProcessTree: options.Tree || options.Force);
            return new KillResult(pid, true, null);
        }
        catch (Exception exception)
        {
            return new KillResult(pid, false, exception.Message);
        }
    }
}
