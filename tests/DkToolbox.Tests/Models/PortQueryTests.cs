using DkToolbox.Core.Models;

namespace DkToolbox.Tests.Models;

public class PortQueryTests
{
    [Fact]
    public void PortQueryShouldCreateInstanceWithTcpProtocol()
    {
        // Arrange & Act
        PortQuery query = new PortQuery(8080, PortProtocol.Tcp);

        // Assert
        Assert.Equal(8080, query.Port);
        Assert.Equal(PortProtocol.Tcp, query.Protocol);
    }

    [Fact]
    public void PortQueryShouldCreateInstanceWithUdpProtocol()
    {
        // Arrange & Act
        PortQuery query = new PortQuery(53, PortProtocol.Udp);

        // Assert
        Assert.Equal(53, query.Port);
        Assert.Equal(PortProtocol.Udp, query.Protocol);
    }

    [Theory]
    [InlineData(80, PortProtocol.Tcp)]
    [InlineData(443, PortProtocol.Tcp)]
    [InlineData(53, PortProtocol.Udp)]
    [InlineData(123, PortProtocol.Udp)]
    public void PortQueryShouldCreateInstanceWithVariousPorts(int port, PortProtocol protocol)
    {
        // Arrange & Act
        PortQuery query = new PortQuery(port, protocol);

        // Assert
        Assert.Equal(port, query.Port);
        Assert.Equal(protocol, query.Protocol);
    }
}
