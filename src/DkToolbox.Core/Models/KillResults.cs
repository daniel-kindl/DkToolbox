namespace DkToolbox.Core.Models;

public enum KillFailureKind
{
    NotFound,
    AccessDenied,
    Unexpected,
}

public abstract record KillResult(int Pid)
{
    public static KillResult Successful(int pid) => new KillSuccess(pid);

    public static KillResult Failed(int pid, KillFailureKind kind, string error) =>
        new KillFailure(pid, kind, error);
}

public sealed record KillSuccess(int Pid) : KillResult(Pid);

public sealed record KillFailure(int Pid, KillFailureKind Kind, string Error) : KillResult(Pid);
