using System.Diagnostics;
using DkToolbox.Core;
using DkToolbox.Core.Abstractions;
using DkToolbox.Core.Models;
using DkToolbox.Core.Services;

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
            Process process = Process.GetProcessById(pid);
            process.Kill(entireProcessTree: options.Tree);
            return KillResult.Successful(pid);
        }
        catch (ArgumentException exception)
        {
            // Process not found (invalid PID or already exited)
            return KillResult.Failed(pid, KillFailureKind.NotFound, exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            // Process exited between GetProcessById and Kill
            return KillResult.Failed(pid, KillFailureKind.NotFound, exception.Message);
        }
        catch (System.ComponentModel.Win32Exception exception)
        {
            // Error code 5 = Access Denied
            return KillResult.Failed(pid,
                exception.NativeErrorCode == 5 ? KillFailureKind.AccessDenied : KillFailureKind.Unexpected,
                exception.Message);
        }
        catch (Exception exception)
        {
            // Unexpected error
            return KillResult.Failed(pid, KillFailureKind.Unexpected, exception.Message);
        }
    }
}
