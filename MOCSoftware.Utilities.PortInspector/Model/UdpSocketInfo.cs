using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MOCSoftware.Utilities.PortInspector.Model
{
    internal sealed class UdpSocketInfo : SocketInfo
    {
        public UdpSocketInfo()
        {
            _protocolType = ProtocolType.Udp;
        }

        public override void BeginScan()
        {
            var task = Task.Factory.StartNew(() => ScanSocket(_cancellationToken), _cancellationToken);
        }

        private void ScanSocket(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                OnSocketCompleted(EventArgs.Empty);
                return;
            }

            try
            {
                using (var client = new UdpClient(HostName, PortNumber))
                {
                    ScanResult = "Scanning";
                    client.Client.ReceiveTimeout = 2500;
                    //client.Client.Connect(HostAddress, PortNumber);
                    Byte[] sendBytes = Encoding.ASCII.GetBytes("Ping");
                    client.Send(sendBytes, sendBytes.Length);
                    var remoteEndpoint = new IPEndPoint(HostAddress, PortNumber);
                    var result = client.Receive(ref remoteEndpoint);
                    ScanResult = "Listening";
                    Status = ScannerResults.PortOpen;
                }
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10054)
                    ScanResult = "Blocked";
                else if (ex.ErrorCode == 10060)
                    ScanResult = "No response within timeout period";
                else
                    ScanResult = string.Format("{0}: {1}", ex.ErrorCode, ex.Message);

                Status = ScannerResults.PortClosed;
            }

            IsScanComplete = true;
            OnSocketCompleted(EventArgs.Empty);
        }
    }
}
