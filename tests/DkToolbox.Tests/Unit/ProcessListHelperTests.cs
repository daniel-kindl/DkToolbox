using DkToolbox.Core;
using DkToolbox.Core.Models;
using DkToolbox.Core.Services;

namespace DkToolbox.Tests.Unit;

public class ProcessListHelperTests
{
    private static readonly ProcessInfo[] TestProcesses =
    [
        new ProcessInfo(1, "chrome", 1000000),
        new ProcessInfo(2, "firefox", 2000000),
        new ProcessInfo(3, "dotnet", 500000),
        new ProcessInfo(4, "code", 1500000),
        new ProcessInfo(5, "notepad", 100000)
    ];

    [Fact]
    public void ApplyQueryShouldSortByNameAscending()
    {
        // Arrange
        ProcessQuery query = new ProcessQuery(null, null, ProcessSort.Name);

        // Act
        IReadOnlyList<ProcessInfo> result = ProcessListHelper.ApplyQuery(TestProcesses, query);

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal("chrome", result[0].Name);
        Assert.Equal("code", result[1].Name);
        Assert.Equal("dotnet", result[2].Name);
        Assert.Equal("firefox", result[3].Name);
        Assert.Equal("notepad", result[4].Name);
    }

    [Fact]
    public void ApplyQueryShouldSortByMemoryDescending()
    {
        // Arrange
        ProcessQuery query = new ProcessQuery(null, null, ProcessSort.Memory);

        // Act
        IReadOnlyList<ProcessInfo> result = ProcessListHelper.ApplyQuery(TestProcesses, query);

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal("firefox", result[0].Name);
        Assert.Equal(2000000, result[0].WorkingSetBytes);
        Assert.Equal("code", result[1].Name);
        Assert.Equal(1500000, result[1].WorkingSetBytes);
        Assert.Equal("chrome", result[2].Name);
        Assert.Equal(1000000, result[2].WorkingSetBytes);
        Assert.Equal("dotnet", result[3].Name);
        Assert.Equal(500000, result[3].WorkingSetBytes);
        Assert.Equal("notepad", result[4].Name);
        Assert.Equal(100000, result[4].WorkingSetBytes);
    }

    [Fact]
    public void ApplyQueryShouldLimitToTopN()
    {
        // Arrange
        ProcessQuery query = new ProcessQuery(null, 3, ProcessSort.Name);

        // Act
        IReadOnlyList<ProcessInfo> result = ProcessListHelper.ApplyQuery(TestProcesses, query);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("chrome", result[0].Name);
        Assert.Equal("code", result[1].Name);
        Assert.Equal("dotnet", result[2].Name);
    }

    [Fact]
    public void ApplyQueryShouldFilterByNameContains()
    {
        // Arrange
        ProcessQuery query = new ProcessQuery("dot", null, ProcessSort.Name);

        // Act
        IReadOnlyList<ProcessInfo> result = ProcessListHelper.ApplyQuery(TestProcesses, query);

        // Assert
        Assert.Single(result);
        Assert.Equal("dotnet", result[0].Name);
    }

    [Fact]
    public void ApplyQueryShouldFilterCaseInsensitive()
    {
        // Arrange
        ProcessQuery query = new ProcessQuery("CHROME", null, ProcessSort.Name);

        // Act
        IReadOnlyList<ProcessInfo> result = ProcessListHelper.ApplyQuery(TestProcesses, query);

        // Assert
        Assert.Single(result);
        Assert.Equal("chrome", result[0].Name);
    }

    [Fact]
    public void ApplyQueryShouldCombineFilterSortAndTop()
    {
        // Arrange - Filter for 'e', sort by memory, take top 2
        ProcessQuery query = new ProcessQuery("e", 2, ProcessSort.Memory);

        // Act
        IReadOnlyList<ProcessInfo> result = ProcessListHelper.ApplyQuery(TestProcesses, query);

        // Assert (matches: chrome, firefox, code, notepad)
        Assert.Equal(2, result.Count);
        Assert.Equal("firefox", result[0].Name); // Highest memory with 'e'
        Assert.Equal("code", result[1].Name);     // Second highest memory with 'e'
    }

    [Fact]
    public void ApplyQueryShouldReturnEmptyListWhenNoMatches()
    {
        // Arrange
        ProcessQuery query = new ProcessQuery("nonexistent", null, ProcessSort.Name);

        // Act
        IReadOnlyList<ProcessInfo> result = ProcessListHelper.ApplyQuery(TestProcesses, query);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ApplyQueryShouldHandleEmptyInput()
    {
        // Arrange
        ProcessInfo[] emptyList = [];
        ProcessQuery query = new ProcessQuery(null, null, ProcessSort.Name);

        // Act
        IReadOnlyList<ProcessInfo> result = ProcessListHelper.ApplyQuery(emptyList, query);

        // Assert
        Assert.Empty(result);
    }
}
