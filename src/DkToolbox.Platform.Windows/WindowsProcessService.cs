using System.Diagnostics;
using DkToolbox.Core.Abstractions;
using DkToolbox.Core.Models;

namespace DkToolbox.Platform.Windows;

public sealed class WindowsProcessService : IProcessService
{
    /// <inheritdoc />
    public IReadOnlyList<ProcessInfo> List(ProcessQuery query)
    {
        var processes = Process.GetProcesses();
        IEnumerable<Process> filtered = processes;
        
        if (!string.IsNullOrWhiteSpace(query.NameContains))
        {
            filtered = filtered.Where(process =>
                process.ProcessName.Contains(query.NameContains, StringComparison.OrdinalIgnoreCase));
        }
            
        var items = filtered.Select(process =>
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

        items = query.Sort switch
        {
            ProcessSort.Memory => items.OrderByDescending(item => item.WorkingSetBytes),
            _ => items.OrderBy(item => item.Name),
        };
        
        if (query.Top is > 0)
        {
            items = items.Take(query.Top.Value);
        }
        
        return items.ToList();
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
