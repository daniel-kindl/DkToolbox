using DkToolbox.Core.Models;

namespace DkToolbox.Tests.Models;

public class PortOwnerInfoTests
{
    [Fact]
    public void PortOwnerInfoShouldCreateInstanceWithTcpProtocol()
    {
        // Arrange & Act
        PortOwnerInfo info = new PortOwnerInfo(PortProtocol.Tcp, "127.0.0.1", 8080, 1234, "test.exe");

        // Assert
        Assert.Equal(PortProtocol.Tcp, info.Protocol);
        Assert.Equal("127.0.0.1", info.LocalAddress);
        Assert.Equal(8080, info.LocalPort);
        Assert.Equal("127.0.0.1:8080", info.LocalEndpoint);
        Assert.Equal(1234, info.Pid);
        Assert.Equal("test.exe", info.ProcessName);
        Assert.Null(info.State);
    }

    [Fact]
    public void PortOwnerInfoShouldCreateInstanceWithUdpProtocol()
    {
        // Arrange & Act
        PortOwnerInfo info = new PortOwnerInfo(PortProtocol.Udp, "0.0.0.0", 53, 5678, "dns.exe");

        // Assert
        Assert.Equal(PortProtocol.Udp, info.Protocol);
        Assert.Equal("0.0.0.0", info.LocalAddress);
        Assert.Equal(53, info.LocalPort);
        Assert.Equal("0.0.0.0:53", info.LocalEndpoint);
        Assert.Equal(5678, info.Pid);
        Assert.Equal("dns.exe", info.ProcessName);
        Assert.Null(info.State);
    }

    [Fact]
    public void PortOwnerInfoShouldIncludeTcpState()
    {
        // Arrange & Act
        PortOwnerInfo info = new PortOwnerInfo(PortProtocol.Tcp, "127.0.0.1", 443, 100, "server.exe", "LISTENING");

        // Assert
        Assert.Equal(PortProtocol.Tcp, info.Protocol);
        Assert.Equal("127.0.0.1", info.LocalAddress);
        Assert.Equal(443, info.LocalPort);
        Assert.Equal("127.0.0.1:443", info.LocalEndpoint);
        Assert.Equal(100, info.Pid);
        Assert.Equal("server.exe", info.ProcessName);
        Assert.Equal("LISTENING", info.State);
    }

    [Theory]
    [InlineData(PortProtocol.Tcp, "192.168.1.1", 443, 100, "server.exe")]
    [InlineData(PortProtocol.Udp, "0.0.0.0", 123, 200, "ntp.exe")]
    public void PortOwnerInfoShouldCreateInstanceWithVariousValues(
        PortProtocol protocol, string address, int port, int pid, string processName)
    {
        // Arrange & Act
        PortOwnerInfo info = new PortOwnerInfo(protocol, address, port, pid, processName);

        // Assert
        Assert.Equal(protocol, info.Protocol);
        Assert.Equal(address, info.LocalAddress);
        Assert.Equal(port, info.LocalPort);
        Assert.Equal($"{address}:{port}", info.LocalEndpoint);
        Assert.Equal(pid, info.Pid);
        Assert.Equal(processName, info.ProcessName);
    }
}
