namespace DkToolbox.Core.Models;

public sealed record ProcessInfo(int Pid, string Name, long WorkingSetBytes);