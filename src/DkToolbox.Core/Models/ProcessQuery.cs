namespace DkToolbox.Core.Models;

public enum ProcessSort
{
    Name, 
    Memory,
}

public sealed record ProcessQuery(string? NameContains, int? Top, ProcessSort Sort);
