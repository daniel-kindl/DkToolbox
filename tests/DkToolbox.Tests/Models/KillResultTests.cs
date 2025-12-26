using DkToolbox.Core.Models;

namespace DkToolbox.Tests.Models;

public class KillResultTests
{
    [Fact]
    public void KillResultShouldCreateInstanceWithSuccessResult()
    {
        // Arrange
        int pid = 1234;

        // Act
        KillResult result = new KillResult(pid, Success: true, Error: null);

        // Assert
        Assert.Equal(pid, result.Pid);
        Assert.True(result.Success);
        Assert.Null(result.Error);
    }

    [Fact]
    public void KillResultShouldCreateInstanceWithFailureResult()
    {
        // Arrange
        int pid = 5678;
        string errorMessage = "Access denied";

        // Act
        KillResult result = new KillResult(pid, Success: false, Error: errorMessage);

        // Assert
        Assert.Equal(pid, result.Pid);
        Assert.False(result.Success);
        Assert.Equal(errorMessage, result.Error);
    }

    [Fact]
    public void KillResultShouldBeEqualWhenPropertiesMatch()
    {
        // Arrange
        KillResult result1 = new KillResult(1234, Success: true, Error: null);
        KillResult result2 = new KillResult(1234, Success: true, Error: null);

        // Act & Assert
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void KillResultShouldNotBeEqualWhenPropertiesDiffer()
    {
        // Arrange
        KillResult result1 = new KillResult(1234, Success: true, Error: null);
        KillResult result2 = new KillResult(1234, Success: false, Error: "Failed");

        // Act & Assert
        Assert.NotEqual(result1, result2);
    }
}
