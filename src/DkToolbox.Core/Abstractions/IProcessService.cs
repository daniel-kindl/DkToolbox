using DkToolbox.Core.Models;

namespace DkToolbox.Core.Abstractions;

public interface IProcessService
{
    IReadOnlyList<ProcessInfo> List(ProcessQuery query);
    KillResult Kill(int pid, KillOptions options);
}