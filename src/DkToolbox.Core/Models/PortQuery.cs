namespace DkToolbox.Core.Models;

public enum PortProtocol
{
    Tcp, 
    Udp,
}

public sealed record PortQuery(int Port, PortProtocol Protocol);