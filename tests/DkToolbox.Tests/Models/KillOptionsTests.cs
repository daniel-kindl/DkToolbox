using DkToolbox.Core.Models;

namespace DkToolbox.Tests.Models;

public class KillOptionsTests
{
    [Fact]
    public void KillOptionsShouldCreateInstanceWithForceAndTree()
    {
        // Arrange & Act
        KillOptions options = new KillOptions(Force: true, Tree: true);

        // Assert
        Assert.True(options.Force);
        Assert.True(options.Tree);
    }

    [Fact]
    public void KillOptionsShouldCreateInstanceWithDefaultValues()
    {
        // Arrange & Act
        KillOptions options = new KillOptions(Force: false, Tree: false);

        // Assert
        Assert.False(options.Force);
        Assert.False(options.Tree);
    }

    [Fact]
    public void KillOptionsShouldBeEqualWhenPropertiesMatch()
    {
        // Arrange
        KillOptions options1 = new KillOptions(Force: true, Tree: false);
        KillOptions options2 = new KillOptions(Force: true, Tree: false);

        // Act & Assert
        Assert.Equal(options1, options2);
    }
}
