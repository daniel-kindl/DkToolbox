using System.Diagnostics;
using DkToolbox.Core.Models;
using DkToolbox.Platform.Windows;

namespace DkToolbox.Tests.Integration;

public class WindowsProcessServiceKillTests
{
    private readonly WindowsProcessService _processService = new();

    [Fact]
    public void KillShouldSucceedForOwnedProcess()
    {
        // Arrange - Start a process we can kill
        Process process = Process.Start(new ProcessStartInfo
        {
            FileName = "notepad.exe",
            UseShellExecute = true,
            CreateNoWindow = false
        })!;

        try
        {
            KillOptions options = new KillOptions(Force: false, Tree: false);

            // Act
            KillResult result = _processService.Kill(process.Id, options);

            // Assert
            KillSuccess success = Assert.IsType<KillSuccess>(result);
            Assert.Equal(process.Id, success.Pid);
        }
        finally
        {
            // Cleanup - ensure process is killed even if test fails
            try
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
            process.Dispose();
        }
    }

    [Fact]
    public void KillShouldSucceedWithTreeOption()
    {
        // Arrange - Start a process we can kill
        Process process = Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c timeout 30",
            UseShellExecute = true,
            CreateNoWindow = true
        })!;

        try
        {
            KillOptions options = new KillOptions(Force: false, Tree: true);

            // Act
            KillResult result = _processService.Kill(process.Id, options);

            // Assert
            KillSuccess success = Assert.IsType<KillSuccess>(result);
            Assert.Equal(process.Id, success.Pid);
        }
        finally
        {
            // Cleanup
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
            process.Dispose();
        }
    }

    [Fact]
    public void KillShouldSucceedWithForceOption()
    {
        // Arrange - Start a process we can kill
        Process process = Process.Start(new ProcessStartInfo
        {
            FileName = "notepad.exe",
            UseShellExecute = true,
            CreateNoWindow = false
        })!;

        try
        {
            KillOptions options = new KillOptions(Force: true, Tree: false);

            // Act
            KillResult result = _processService.Kill(process.Id, options);

            // Assert
            KillSuccess success = Assert.IsType<KillSuccess>(result);
            Assert.Equal(process.Id, success.Pid);
        }
        finally
        {
            // Cleanup
            try
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
            process.Dispose();
        }
    }

    [Fact]
    public void KillShouldReturnNotFoundForInvalidPid()
    {
        // Arrange
        const int invalidPid = 999999;
        KillOptions options = new KillOptions(Force: false, Tree: false);

        // Act
        KillResult result = _processService.Kill(invalidPid, options);

        // Assert
        KillFailure failure = Assert.IsType<KillFailure>(result);
        Assert.Equal(KillFailureKind.NotFound, failure.Kind);
        Assert.Equal(invalidPid, failure.Pid);
        Assert.NotEmpty(failure.Error);
    }

    [Fact]
    public void KillShouldReturnNotFoundForAlreadyExitedProcess()
    {
        // Arrange - Start and immediately kill a process
        Process process = Process.Start(new ProcessStartInfo
        {
            FileName = "notepad.exe",
            UseShellExecute = true,
            CreateNoWindow = false
        })!;
        
        int pid = process.Id;
        process.Kill();
        process.WaitForExit(5000);
        process.Dispose();

        Thread.Sleep(100); // Ensure process is fully cleaned up

        KillOptions options = new KillOptions(Force: false, Tree: false);

        // Act
        KillResult result = _processService.Kill(pid, options);

        // Assert
        KillFailure failure = Assert.IsType<KillFailure>(result);
        Assert.Equal(KillFailureKind.NotFound, failure.Kind);
        Assert.Equal(pid, failure.Pid);
    }
}
