namespace DkToolbox.Core.Models;

public enum KillFailureKind
{
    NotFound,
    AccessDenied,
    Unexpected
}

public sealed record KillResult(int Pid, bool Success, KillFailureKind? FailureKind, string? Error)
{
    public static KillResult Successful(int pid) => new(pid, true, null, null);
    
    public static KillResult Failed(int pid, KillFailureKind kind, string error) => 
        new(pid, false, kind, error);
}
