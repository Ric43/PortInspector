using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MOCSoftware.Utilities.PortInspector.Model
{
    internal interface ISocketInfo
    {
        string HostName { get; set; }
        IPAddress HostAddress { get; set; }
        ProtocolType Protocol { get; }
        CancellationToken CancellationToken { set; }
        string ScanResult { get; set; }
        bool IsScanComplete { get; set; }
        int PortNumber { get; set; }

        event EventHandler SocketCompleted;

        void BeginScan();
    }
}
