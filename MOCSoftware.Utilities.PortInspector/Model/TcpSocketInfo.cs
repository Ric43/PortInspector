using MOCSoftware.Utilities.PortInspector.Properties;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MOCSoftware.Utilities.PortInspector.Model
{
    internal sealed class TcpSocketInfo : SocketInfo
    {
        public TcpSocketInfo()
        {
            _protocolType = ProtocolType.Tcp;
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

            using (var socket = new Socket(HostAddress.AddressFamily, SocketType.Stream, Protocol))
            {
                try
                {
                    socket.Connect(HostAddress, PortNumber);
                    ScanResult = Resources.TcpSocketInfo_ScanResultOkMessage;
                    Status = ScannerResults.PortOpen;
                }
                catch(SocketException ex)
                {
                    ScanResult = ex.SocketErrorCode.ToString();
                    Status = ScannerResults.PortClosed;
                }
            }

            IsScanComplete = true;
            OnSocketCompleted(EventArgs.Empty);
        }
    }
}
