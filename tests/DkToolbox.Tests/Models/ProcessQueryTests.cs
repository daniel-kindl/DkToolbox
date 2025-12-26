using DkToolbox.Core.Models;

namespace DkToolbox.Tests.Models;

public class ProcessQueryTests
{
    [Fact]
    public void ProcessQueryShouldCreateInstanceWithAllParameters()
    {
        // Arrange
        string nameContains = "chrome";
        int top = 10;
        ProcessSort sort = ProcessSort.Memory;

        // Act
        ProcessQuery query = new ProcessQuery(nameContains, top, sort);

        // Assert
        Assert.Equal(nameContains, query.NameContains);
        Assert.Equal(top, query.Top);
        Assert.Equal(sort, query.Sort);
    }

    [Fact]
    public void ProcessQueryShouldCreateInstanceWithNullNameContains()
    {
        // Arrange & Act
        ProcessQuery query = new ProcessQuery(null, 5, ProcessSort.Name);

        // Assert
        Assert.Null(query.NameContains);
        Assert.Equal(5, query.Top);
        Assert.Equal(ProcessSort.Name, query.Sort);
    }

    [Fact]
    public void ProcessQueryShouldCreateInstanceWithNullTop()
    {
        // Arrange & Act
        ProcessQuery query = new ProcessQuery("test", null, ProcessSort.Memory);

        // Assert
        Assert.Equal("test", query.NameContains);
        Assert.Null(query.Top);
        Assert.Equal(ProcessSort.Memory, query.Sort);
    }
}
