using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MOCSoftware.Utilities.PortInspector.Model
{
    public enum ScannerResults
    {
        Scanning = 0,
        PortOpen = 1,
        PortClosed = 2
    }

    internal abstract class SocketInfo : INotifyPropertyChanged, ISocketInfo
    {

        public event EventHandler SocketCompleted;

        public event PropertyChangedEventHandler PropertyChanged;
        protected ProtocolType _protocolType;

        private string _scanResult;
        private bool _isScanComplete;

        private ScannerResults _status = ScannerResults.Scanning;
        public ScannerResults Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public IPAddress HostAddress { get; set; }
        public int PortNumber { get; set; }
        public ProtocolType Protocol
        {
            get
            {
                return _protocolType;
            }
        }
        public string HostName { get; set; }

        protected CancellationToken _cancellationToken;
        public CancellationToken CancellationToken
        {
            set
            {
                _cancellationToken = value;
            }
        }

        public string ScanResult
        {
            get
            {
                return _scanResult;
            }
            set
            {
                _scanResult = value;
                OnPropertyChanged("ScanResult");
            }
        }

        public bool IsScanComplete
        {
            get
            {
                return _isScanComplete;
            }
            set
            {
                _isScanComplete = value;
                OnPropertyChanged("IsScanComplete");
            }
        }

        protected void OnSocketCompleted(EventArgs e)
        {
            if (SocketCompleted != null)
                SocketCompleted(this, e);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public abstract void BeginScan();
    }
}
