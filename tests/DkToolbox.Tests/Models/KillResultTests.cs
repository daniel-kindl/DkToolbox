using DkToolbox.Core.Models;

namespace DkToolbox.Tests.Models;

public class KillResultTests
{
    [Fact]
    public void KillResultShouldCreateSuccessfulResult()
    {
        // Arrange
        const int pid = 1234;

        // Act
        KillResult result = KillResult.Successful(pid);

        // Assert
        Assert.Equal(pid, result.Pid);
        Assert.IsType<KillSuccess>(result);
    }

    [Fact]
    public void KillResultShouldCreateFailedResultWithNotFound()
    {
        // Arrange
        const int pid = 5678;
        const string errorMessage = "Process not found";

        // Act
        KillResult result = KillResult.Failed(pid, KillFailureKind.NotFound, errorMessage);

        // Assert
        Assert.Equal(pid, result.Pid);
        KillFailure failure = Assert.IsType<KillFailure>(result);
        Assert.Equal(KillFailureKind.NotFound, failure.Kind);
        Assert.Equal(errorMessage, failure.Error);
    }

    [Fact]
    public void KillResultShouldCreateFailedResultWithAccessDenied()
    {
        // Arrange
        const int pid = 9012;
        const string errorMessage = "Access denied";

        // Act
        KillResult result = KillResult.Failed(pid, KillFailureKind.AccessDenied, errorMessage);

        // Assert
        Assert.Equal(pid, result.Pid);
        KillFailure failure = Assert.IsType<KillFailure>(result);
        Assert.Equal(KillFailureKind.AccessDenied, failure.Kind);
        Assert.Equal(errorMessage, failure.Error);
    }

    [Fact]
    public void KillResultShouldCreateFailedResultWithUnexpected()
    {
        // Arrange
        const int pid = 3456;
        const string errorMessage = "Unexpected error occurred";

        // Act
        KillResult result = KillResult.Failed(pid, KillFailureKind.Unexpected, errorMessage);

        // Assert
        Assert.Equal(pid, result.Pid);
        KillFailure failure = Assert.IsType<KillFailure>(result);
        Assert.Equal(KillFailureKind.Unexpected, failure.Kind);
        Assert.Equal(errorMessage, failure.Error);
    }

    [Fact]
    public void KillResultShouldBeEqualWhenPropertiesMatch()
    {
        // Arrange
        KillResult result1 = KillResult.Successful(1234);
        KillResult result2 = KillResult.Successful(1234);

        // Act & Assert
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void KillResultShouldNotBeEqualWhenPropertiesDiffer()
    {
        // Arrange
        KillResult result1 = KillResult.Successful(1234);
        KillResult result2 = KillResult.Failed(1234, KillFailureKind.NotFound, "Failed");

        // Act & Assert
        Assert.NotEqual(result1, result2);
    }
}
