using DkToolbox.Core.Models;
using DkToolbox.Platform.Windows;

namespace DkToolbox.Tests.Integration;

public class WindowsProcessServiceTests
{
    private readonly WindowsProcessService _processService = new();

    [Fact]
    public void ListShouldReturnProcessesWhenCalledWithoutFilters()
    {
        // Arrange
        ProcessQuery query = new ProcessQuery(null, null, ProcessSort.Name);

        // Act
        IReadOnlyList<ProcessInfo> processes = _processService.List(query);

        // Assert
        Assert.NotEmpty(processes);
        Assert.All(processes, process =>
        {
            Assert.True(process.Pid >= 0);
            Assert.NotNull(process.Name);
            Assert.True(process.WorkingSetBytes >= 0);
        });
    }

    [Fact]
    public void ListShouldFilterByNameWhenNameContainsProvided()
    {
        // Arrange
        ProcessQuery query = new ProcessQuery("svchost", null, ProcessSort.Name);

        // Act
        IReadOnlyList<ProcessInfo> processes = _processService.List(query);

        // Assert
        Assert.All(processes, p =>
            Assert.Contains("svchost", p.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ListShouldLimitResultsWhenTopProvided()
    {
        // Arrange
        const int topCount = 5;
        ProcessQuery query = new ProcessQuery(null, topCount, ProcessSort.Memory);

        // Act
        IReadOnlyList<ProcessInfo> processes = _processService.List(query);

        // Assert
        Assert.True(processes.Count <= topCount);
    }

    [Fact]
    public void ListShouldSortByMemoryWhenMemorySortSpecified()
    {
        // Arrange
        ProcessQuery query = new ProcessQuery(null, 10, ProcessSort.Memory);

        // Act
        IReadOnlyList<ProcessInfo> processes = _processService.List(query);

        // Assert
        for (int i = 0; i < processes.Count - 1; i++)
        {
            Assert.True(processes[i].WorkingSetBytes >= processes[i + 1].WorkingSetBytes);
        }
    }

    [Fact]
    public void ListShouldSortByNameWhenNameSortSpecified()
    {
        // Arrange
        ProcessQuery query = new ProcessQuery(null, 10, ProcessSort.Name);

        // Act
        IReadOnlyList<ProcessInfo> processes = _processService.List(query);

        // Assert
        for (int i = 0; i < processes.Count - 1; i++)
        {
            Assert.True(string.Compare(processes[i].Name, processes[i + 1].Name, StringComparison.OrdinalIgnoreCase) <= 0);
        }
    }

    [Fact]
    public void KillShouldReturnFailureWhenProcessDoesNotExist()
    {
        // Arrange
        const int nonExistentPid = 999999;
        KillOptions options = new KillOptions(Force: false, Tree: false);

        // Act
        KillResult result = _processService.Kill(nonExistentPid, options);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(KillFailureKind.NotFound, result.FailureKind);
        Assert.NotNull(result.Error);
        Assert.Equal(nonExistentPid, result.Pid);
    }
}
