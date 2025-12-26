namespace DkToolbox.Core.Models;

public sealed record PortOwnerInfo(PortProtocol Protocol, string LocalEndpoint, int Pid, string ProcessName);