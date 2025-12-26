using DkToolbox.Core.Models;

namespace DkToolbox.Core;

public static class ProcessListHelper
{
    public static IReadOnlyList<ProcessInfo> ApplyQuery(
        IEnumerable<ProcessInfo> processes,
        ProcessQuery query)
    {
        IEnumerable<ProcessInfo> filtered = processes;

        if (!string.IsNullOrWhiteSpace(query.NameContains))
        {
            filtered = filtered.Where(p =>
                p.Name.Contains(query.NameContains, StringComparison.OrdinalIgnoreCase));
        }

        IEnumerable<ProcessInfo> sorted = query.Sort switch
        {
            ProcessSort.Memory => filtered.OrderByDescending(p => p.WorkingSetBytes),
            _ => filtered.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
        };

        if (query.Top is > 0)
        {
            sorted = sorted.Take(query.Top.Value);
        }

        return sorted.ToList();
    }
}
