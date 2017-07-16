using System.Windows.Input;
using MOCSoftware.Utilities.PortInspector.Communication;
using MOCSoftware.Utilities.PortInspector.Creator;
using MOCSoftware.Utilities.PortInspector.Model;
using MOCSoftware.Utilities.PortInspector.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace MOCSoftware.Utilities.PortInspector.ViewModel
{
    internal class ScannerResultsViewModel : ViewModelBase
    {
        public MainMenuViewModel MainMenu { get; set; }
        private CancellationTokenSource _tokenSource;
        private readonly SocketInfoFactory _factory = new SocketInfoFactory();

        private int _numberCompleted;

        private readonly ObservableCollection<ISocketInfo> _scannerResults = new ObservableCollection<ISocketInfo>();
        public ObservableCollection<ISocketInfo> ScannerResults
        {
            get
            {
                return _scannerResults;
            }
        }

        private ICommand _scanInitiatedCommand;
        internal ICommand ScanInitiatedCommand
        {
            get { return _scanInitiatedCommand ?? (_scanInitiatedCommand = new RelayCommand(e => OnScanInitiated((ScanInitiatedMessage)e))); }
        }

        private ICommand _scanAbortCommand;
        internal ICommand ScanAbortCommand
        {
            get { return _scanAbortCommand ?? (_scanAbortCommand = new RelayCommand(e => OnScanAborted())); }
        }

        public ScannerResultsViewModel()
        {
            App.Mediator.Subscribe(App._EVENT_HOSTLIST_SCANSTART, this, ScanInitiatedCommand);
            App.Mediator.Subscribe(App._EVENT_HOSTLIST_SCANABORT, this, ScanAbortCommand);
        }

        ~ScannerResultsViewModel()
        {
            App.Mediator.Unsubscribe(App._EVENT_HOSTLIST_SCANSTART, this, ScanInitiatedCommand);
            App.Mediator.Unsubscribe(App._EVENT_HOSTLIST_SCANABORT, this, ScanAbortCommand);
        }

        public void Initialise()
        {
        }

        internal void OnScanInitiated(ScanInitiatedMessage message)
        {
            ScannerResults.Clear();
            _tokenSource = new CancellationTokenSource();
            _numberCompleted = 0;
            App.Mediator.Publish(App._EVENT_STATUSUPDATE,
                new StatusUpdateMessage{ ProgressValue = 0, ShowBusy = true}
                );
            message.HostsToScan.ForEach(e => ExtractHosts(e, message.PortsToScan));
        }

        private void ExtractHosts(HostInfo host, List<PortInfo> ports)
        {
            ports.ForEach(e => ExtractPorts(host, e));
        }

        private void ExtractPorts(HostInfo host, PortInfo port)
        {
            var socket = _factory.CreateSocketInfo(host.HostName, host.HostAddress, port.PortNumber,
                port.Protocol, _tokenSource.Token);
            socket.BeginScan();
            socket.SocketCompleted += new EventHandler(OnSocketCompleted);
            _scannerResults.Add(socket);
        }

        private void OnScanAborted()
        {
            _tokenSource.Cancel();
        }

        private void OnSocketCompleted(object sender, EventArgs e)
        {
            _numberCompleted++;
            var progressValue = (int) (((decimal)_numberCompleted/(decimal)_scannerResults.Count)*100);
            App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage { ProgressValue = progressValue});
            if (_numberCompleted == _scannerResults.Count)
            {
                App.Mediator.Publish(App._EVENT_SCANNER_COMPLETE);
            }
        }
    }
}
