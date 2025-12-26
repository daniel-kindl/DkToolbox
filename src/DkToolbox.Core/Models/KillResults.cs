namespace DkToolbox.Core.Models;

public sealed record KillResult(int Pid, bool Success, string? Error);