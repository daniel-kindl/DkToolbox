using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using DkToolbox.Core.Abstractions;
using DkToolbox.Core.Models;

namespace DkToolbox.Platform.Windows;

public sealed partial class WindowsPortService : IPortService
{
    public IReadOnlyList<PortOwnerInfo> WhoOwns(PortQuery query)
    {
        return query.Protocol switch
        {
            PortProtocol.Tcp => GetTcpOwners(query.Port),
            PortProtocol.Udp => GetUdpOwners(query.Port),
            _ => []
        };
    }

    private static List<PortOwnerInfo> GetTcpOwners(int port)
    {
        List<PortOwnerInfo> owners = [];
        Dictionary<string, (int Pid, string? State)> endpointToPidAndState = GetTcpPidMapping();

        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
        TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();

        foreach (TcpConnectionInformation conn in connections)
        {
            if (conn.LocalEndPoint.Port == port)
            {
                string key = conn.LocalEndPoint.ToString();
                (int pid, string? state) = endpointToPidAndState.GetValueOrDefault(key, (0, null));
                string processName = GetProcessName(pid);
                
                owners.Add(new PortOwnerInfo(
                    PortProtocol.Tcp,
                    conn.LocalEndPoint.Address.ToString(),
                    conn.LocalEndPoint.Port,
                    pid,
                    processName,
                    state ?? conn.State.ToString()));
            }
        }

        IPEndPoint[] listeners = properties.GetActiveTcpListeners();
        foreach (IPEndPoint listener in listeners)
        {
            if (listener.Port == port)
            {
                string key = listener.ToString();
                (int pid, string? state) = endpointToPidAndState.GetValueOrDefault(key, (0, null));
                string processName = GetProcessName(pid);

                owners.Add(new PortOwnerInfo(
                    PortProtocol.Tcp,
                    listener.Address.ToString(),
                    listener.Port,
                    pid,
                    processName,
                    state ?? "LISTENING"));
            }
        }

        return owners;
    }

    private static List<PortOwnerInfo> GetUdpOwners(int port)
    {
        List<PortOwnerInfo> owners = [];
        Dictionary<string, int> endpointToPid = GetUdpPidMapping();

        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] endpoints = properties.GetActiveUdpListeners();

        foreach (IPEndPoint endpoint in endpoints)
        {
            if (endpoint.Port == port)
            {
                string key = endpoint.ToString();
                int pid = endpointToPid.GetValueOrDefault(key, 0);
                string processName = GetProcessName(pid);

                owners.Add(new PortOwnerInfo(
                    PortProtocol.Udp,
                    endpoint.Address.ToString(),
                    endpoint.Port,
                    pid,
                    processName));
            }
        }

        return owners;
    }

    private static Dictionary<string, (int Pid, string? State)> GetTcpPidMapping()
    {
        Dictionary<string, (int, string?)> mapping = new();

        try
        {
            int bufferSize = 0;
            _ = GetExtendedTcpTable(IntPtr.Zero, ref bufferSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);

            IntPtr tcpTablePtr = Marshal.AllocHGlobal(bufferSize);
            try
            {
                if (GetExtendedTcpTable(tcpTablePtr, ref bufferSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0) == 0)
                {
                    MIB_TCPTABLE_OWNER_PID tcpTable = Marshal.PtrToStructure<MIB_TCPTABLE_OWNER_PID>(tcpTablePtr);
                    IntPtr rowPtr = tcpTablePtr + Marshal.SizeOf(tcpTable.dwNumEntries);

                    for (int i = 0; i < tcpTable.dwNumEntries; i++)
                    {
                        MIB_TCPROW_OWNER_PID row = Marshal.PtrToStructure<MIB_TCPROW_OWNER_PID>(rowPtr);
                        
                        IPAddress localAddr = new IPAddress(row.localAddr);
                        int localPort = (row.localPort >> 8) | ((row.localPort << 8) & 0xFF00);
                        string endpoint = $"{localAddr}:{localPort}";
                        string? state = GetTcpStateName(row.state);
                        
                        mapping.TryAdd(endpoint, (row.owningPid, state));
                        
                        rowPtr += Marshal.SizeOf<MIB_TCPROW_OWNER_PID>();
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(tcpTablePtr);
            }
        }
        catch
        {
            // If native API fails, return empty mapping
        }

        return mapping;
    }

    private static Dictionary<string, int> GetUdpPidMapping()
    {
        Dictionary<string, int> mapping = new();

        try
        {
            int bufferSize = 0;
            _ = GetExtendedUdpTable(IntPtr.Zero, ref bufferSize, true, AF_INET, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);

            IntPtr udpTablePtr = Marshal.AllocHGlobal(bufferSize);
            try
            {
                if (GetExtendedUdpTable(udpTablePtr, ref bufferSize, true, AF_INET, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0) == 0)
                {
                    MIB_UDPTABLE_OWNER_PID udpTable = Marshal.PtrToStructure<MIB_UDPTABLE_OWNER_PID>(udpTablePtr);
                    IntPtr rowPtr = udpTablePtr + Marshal.SizeOf(udpTable.dwNumEntries);

                    for (int i = 0; i < udpTable.dwNumEntries; i++)
                    {
                        MIB_UDPROW_OWNER_PID row = Marshal.PtrToStructure<MIB_UDPROW_OWNER_PID>(rowPtr);
                        
                        IPAddress localAddr = new IPAddress(row.localAddr);
                        int localPort = (row.localPort >> 8) | ((row.localPort << 8) & 0xFF00);
                        string endpoint = $"{localAddr}:{localPort}";
                        
                        mapping.TryAdd(endpoint, row.owningPid);
                        
                        rowPtr += Marshal.SizeOf<MIB_UDPROW_OWNER_PID>();
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(udpTablePtr);
            }
        }
        catch
        {
            // If native API fails, return empty mapping
        }

        return mapping;
    }

    private static string? GetTcpStateName(int state)
    {
        return state switch
        {
            1 => "CLOSED",
            2 => "LISTENING",
            3 => "SYN_SENT",
            4 => "SYN_RCVD",
            5 => "ESTABLISHED",
            6 => "FIN_WAIT1",
            7 => "FIN_WAIT2",
            8 => "CLOSE_WAIT",
            9 => "CLOSING",
            10 => "LAST_ACK",
            11 => "TIME_WAIT",
            12 => "DELETE_TCB",
            _ => null
        };
    }

    private static string GetProcessName(int pid)
    {
        if (pid == 0)
        {
            return "Unknown";
        }

        try
        {
            Process process = Process.GetProcessById(pid);
            return process.ProcessName;
        }
        catch
        {
            return "Unknown";
        }
    }

    // P/Invoke declarations
    private const int AF_INET = 2;

    [LibraryImport("iphlpapi.dll", SetLastError = true)]
    private static partial int GetExtendedTcpTable(IntPtr tcpTable, ref int size, [MarshalAs(UnmanagedType.Bool)] bool sort, int ipVersion, TCP_TABLE_CLASS tableClass, int reserved);

    [LibraryImport("iphlpapi.dll", SetLastError = true)]
    private static partial int GetExtendedUdpTable(IntPtr udpTable, ref int size, [MarshalAs(UnmanagedType.Bool)] bool sort, int ipVersion, UDP_TABLE_CLASS tableClass, int reserved);

    private enum TCP_TABLE_CLASS
    {
        TCP_TABLE_OWNER_PID_ALL = 5
    }

    private enum UDP_TABLE_CLASS
    {
        UDP_TABLE_OWNER_PID = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_TCPTABLE_OWNER_PID
    {
        public int dwNumEntries;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_TCPROW_OWNER_PID
    {
        public int state;
        public uint localAddr;
        public int localPort;
        public uint remoteAddr;
        public int remotePort;
        public int owningPid;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_UDPTABLE_OWNER_PID
    {
        public int dwNumEntries;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_UDPROW_OWNER_PID
    {
        public uint localAddr;
        public int localPort;
        public int owningPid;
    }
}
