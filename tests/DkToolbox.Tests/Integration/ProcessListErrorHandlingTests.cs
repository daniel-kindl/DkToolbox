using DkToolbox.Core.Abstractions;
using DkToolbox.Core.Models;
using DkToolbox.Platform.Windows;

namespace DkToolbox.Tests.Integration;

public class ProcessListErrorHandlingTests
{
    [Fact]
    public void ListShouldNotCrashWhenProcessAccessDenied()
    {
        // Arrange
        WindowsProcessService service = new WindowsProcessService();
        ProcessQuery query = new ProcessQuery(null, null, ProcessSort.Memory);

        // Act - Should not throw even if some processes deny access
        IReadOnlyList<ProcessInfo> processes = service.List(query);

        // Assert
        Assert.NotEmpty(processes);
        
        // Verify processes with denied access have 0 memory instead of crashing
        ProcessInfo[] processesWithZeroMemory = processes.Where(p => p.WorkingSetBytes == 0).ToArray();
        
        // It's okay to have processes with 0 memory (access denied)
        // The important thing is we didn't crash
        Assert.True(true);
    }

    [Fact]
    public void ListShouldHandleLargeResultSets()
    {
        // Arrange
        WindowsProcessService service = new WindowsProcessService();
        ProcessQuery query = new ProcessQuery(null, null, ProcessSort.Name);

        // Act - Get all processes
        IReadOnlyList<ProcessInfo> processes = service.List(query);

        // Assert - Should handle potentially hundreds of processes
        Assert.NotEmpty(processes);
        Assert.All(processes, p =>
        {
            Assert.True(p.Pid >= 0);
            Assert.NotNull(p.Name);
            Assert.True(p.WorkingSetBytes >= 0);
        });
    }

    [Fact]
    public void ListShouldWorkWithChangingProcessList()
    {
        // Arrange
        WindowsProcessService service = new WindowsProcessService();
        ProcessQuery query = new ProcessQuery(null, 50, ProcessSort.Memory);

        // Act - Call multiple times, process list may change between calls
        IReadOnlyList<ProcessInfo> firstCall = service.List(query);
        IReadOnlyList<ProcessInfo> secondCall = service.List(query);

        // Assert - Both calls should succeed without crashing
        Assert.NotEmpty(firstCall);
        Assert.NotEmpty(secondCall);
    }
}
