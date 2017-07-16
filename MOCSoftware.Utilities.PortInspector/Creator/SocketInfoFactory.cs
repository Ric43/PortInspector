using MOCSoftware.Utilities.PortInspector.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MOCSoftware.Utilities.PortInspector.Creator
{
    internal sealed class SocketInfoFactory
    {
        internal ISocketInfo CreateSocketInfo(string hostName, IPAddress hostAddress, short portNumber, ProtocolType protocol, CancellationToken token)
        {
            ISocketInfo socketInfo;
            switch (protocol)
            {
                case ProtocolType.Tcp:
                    socketInfo = new TcpSocketInfo();
                    break;
                case ProtocolType.Udp:
                    socketInfo = new UdpSocketInfo();
                    break;
                default:
                    throw new InvalidOperationException("Invalid protocol type specified for socket.");
            }

            socketInfo.HostName = hostName;
            socketInfo.HostAddress = hostAddress;
            socketInfo.PortNumber = portNumber;
            socketInfo.CancellationToken = token;
            return socketInfo;
        }
    }
}
