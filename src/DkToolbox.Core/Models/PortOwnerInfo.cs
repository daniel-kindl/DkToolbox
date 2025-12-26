namespace DkToolbox.Core.Models;

public sealed record PortOwnerInfo(
    PortProtocol Protocol,
    string LocalAddress,
    int LocalPort,
    int Pid,
    string ProcessName,
    string? State = null)
{
    public string LocalEndpoint => $"{LocalAddress}:{LocalPort}";
}