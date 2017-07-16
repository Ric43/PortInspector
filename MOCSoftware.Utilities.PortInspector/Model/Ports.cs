using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using MOCSoftware.Utilities.PortInspector.Persistence;
using MOCSoftware.Utilities.PortInspector.Utility;
using MOCSoftware.Utilities.PortInspector.Communication;
using MOCSoftware.Utilities.PortInspector.Properties;
using System.Net.Sockets;

namespace MOCSoftware.Utilities.PortInspector.Model
{
    public sealed class Ports : ObservableCollection<PortInfo>
    {
        internal delegate void DataLoadedCallbackDelegate(IAsyncResult result);
        public delegate void LoadDataFromFile();
        private delegate void DataLoadDelegate(LoadDataFromFile fileLoadMethod);
        private delegate void DataSaveDelegate();

        private IPortDataPersistor DataPersistor { get; set; }
        private List<PortInfo> _portInfoList = new List<PortInfo>();

        private readonly PortFilter _portFilter;

        internal Ports()
        {
            DataPersistor = new PortDataPersistor {PortInfoList = _portInfoList};
            _portFilter = new PortFilter();
        }

        internal void AddPort(PortInfo portfInfo)
        {
            var portsToAdd = new List<PortInfo>();
            if (portfInfo.Protocol == ProtocolType.Unspecified)
            {
                portsToAdd.Add(new PortInfo(portfInfo, ProtocolType.Tcp));
                portsToAdd.Add(new PortInfo(portfInfo, ProtocolType.Udp));
            }
            else
            {
                portsToAdd.Add(new PortInfo(portfInfo, portfInfo.Protocol));
            }

            var messages = new StringBuilder();

            foreach (var portToAdd in portsToAdd)
            {
                var existingPort = _portInfoList.FirstOrDefault(e => e.PortNumber == portToAdd.PortNumber && e.Protocol == portToAdd.Protocol);
                if (existingPort == null)
                {
                    _portInfoList.Add(portToAdd);
                    SetFilter(_portFilter.CurrentFilter, _portFilter.CurrentProtocol);
                }
                else
                {
                    messages.AppendFormat(Resources.Error_DuplicatePort_Message, existingPort.PortName, existingPort.PortNumber, existingPort.Protocol);
                }
            }

            if (messages.ToString().Length > 0)
                PortExistsError(messages.ToString());
        }

        internal void EditPort(PortInfo selectedPort, PortInfo updatedPort)
        {
            if (_portInfoList.Contains(selectedPort))
            {
                var existingPort = _portInfoList.FirstOrDefault(e => e.PortNumber == updatedPort.PortNumber && e.Protocol == updatedPort.Protocol
                    && updatedPort.PortNumber != selectedPort.PortNumber && updatedPort.Protocol != selectedPort.Protocol);
                if (existingPort != null)
                {
                    App.Mediator.Publish(App._EVENT_ERROR,
                        new ErrorMessage { ErrorTitle = Resources.Error_DuplicatePort_Title,
                        ErrorText = string.Format(Resources.Error_DuplicatePort_Message, existingPort.PortName, existingPort.PortNumber, existingPort.Protocol) });
                    return;
                }
                _portInfoList[_portInfoList.IndexOf(selectedPort)] = updatedPort;
                SetFilter(_portFilter.CurrentFilter, _portFilter.CurrentProtocol);
            }
        }

        private void PortExistsError(string portName)
        {
            App.Mediator.Publish(App._EVENT_ERROR,
                                    new ErrorMessage { ErrorTitle = Resources.Error_DuplicatePort_Title,
                                    ErrorText = portName });
        }

        internal void LoadData(LoadDataFromFile fileLoadMethod)
        {
            fileLoadMethod.Invoke();
            _portInfoList = DataPersistor.PortInfoList;
            _portInfoList.Sort();
            SetPortList();
        }

        internal IAsyncResult LoadDataAsync(DataLoadedCallbackDelegate onLoadedCallback)
        {
            return LoadDataAsync(onLoadedCallback, LoadFromSources.TryDataFileFirstThenServicesFile);
        }

        internal IAsyncResult LoadDataAsync(DataLoadedCallbackDelegate onLoadedCallback, LoadFromSources loadFrom)
        {
            var loader = new DataLoadDelegate(LoadData);
            var loaded = new AsyncCallback(onLoadedCallback);
            switch (loadFrom)
            {
                case LoadFromSources.TryDataFileFirstThenServicesFile:
                    return loader.BeginInvoke(DataPersistor.LoadPortData, loaded, loader);
                case LoadFromSources.DataFile:
                    return loader.BeginInvoke(DataPersistor.LoadApplicationDataFromDataFile, loaded, loader);
                default:
                    return loader.BeginInvoke(DataPersistor.LoadApplicationDataFromServicesFile, loaded, loader);
            }
        }

        internal void SaveData()
        {
            DataPersistor.SavePortData();
        }

        internal IAsyncResult SaveDataAsync(DataLoadedCallbackDelegate onSavedCallback)
        {
            var saver = new DataSaveDelegate(SaveData);
            var saved = new AsyncCallback(onSavedCallback);
            return saver.BeginInvoke(saved, saver);
        }

        internal void SetFilter(string filter, PortInspectorProtocols protocol)
        {
            var filteredList = _portFilter.FilterPorts(filter, protocol, _portInfoList);
            if (filteredList != null)
            {
                filteredList.Sort();
                Clear();
                foreach (var port in filteredList)
                    Add(port);
                _portInfoList.Where(port => port.IsSelected && !Contains(port))
                    .ToList().ForEach(deselect => deselect.IsSelected = false);
                var selectedPorts = this.Where(port => port.IsSelected);
                App.Mediator.Publish(App._EVENT_PORT_PORTSELECTED, new PortSelectedMessage { SelectedPortList = selectedPorts.ToList() });
            }
        }

        private void SetPortList()
        {
            _portInfoList.ForEach(port => Application.Current.Dispatcher.Invoke((Action)(() => Add(port))));
        }

        internal void ClearList()
        {
            Clear();
            _portInfoList.Clear();
        }
    }
}
