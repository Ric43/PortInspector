using MOCSoftware.Utilities.PortInspector.Communication;
using System;
using System.ComponentModel;
using System.Net.Sockets;

namespace MOCSoftware.Utilities.PortInspector.Model
{
    [Serializable]
    public sealed class PortInfo : IEquatable<PortInfo>, INotifyPropertyChanged, IComparable<PortInfo>
    {
        private string _portName;
        private ProtocolType _protocol;
        private short _portNumber;
        private string _alias;
        private string _descrption;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        private bool _isSelected;

        public string PortName
        {
            get
            {
                return _portName;
            }
            private set
            {
                _portName = value;
                OnPropertyChanged("PortName");
            }
        }

        public ProtocolType Protocol
        {
            get { return _protocol; }
            internal set
            {
                _protocol = value;
                OnPropertyChanged("Protocol");
            }
        }

        public short PortNumber
        {
            get
            {
                return _portNumber;
            }
            private set
            {
                _portNumber = value;
                OnPropertyChanged("PortNumber");
            }
        }

        public string Alias
        {
            get { return _alias; }
            private set
            {
                _alias = value;
                OnPropertyChanged("Alias");
            }
        }

        public string Description
        {
            get { return _descrption; }
            private set
            {
                _descrption = value;
                OnPropertyChanged("Description");
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
                App.Mediator.Publish(App._EVENT_PORT_PORTSELECTED, new PortSelectedMessage { Port = this });
            }
        }

        internal PortInfo(string portName, string protocol, short portNumber, string alias, string description, bool isSelected)
        {
            PortName = portName;
            ProtocolType protocolTemp;
            Protocol = Enum.TryParse(protocol, true, out protocolTemp) ? protocolTemp : ProtocolType.Unspecified;
            PortNumber = portNumber;
            Alias = alias;
            Description = description;
            IsSelected = isSelected;
        }

        internal PortInfo(PortInfo portInfo, ProtocolType protocolType = ProtocolType.Unspecified)
        {
            Alias = portInfo.Alias;
            Description = portInfo.Description;
            PortName = portInfo.PortName;
            PortNumber = portInfo.PortNumber;
            Protocol = protocolType;
        }

        public bool Equals(PortInfo other)
        {
            return (other.Protocol == Protocol && other.PortNumber == PortNumber);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int CompareTo(PortInfo other)
        {
            if (_portNumber > other.PortNumber)
                return 1;
            if (_portNumber < other.PortNumber)
                return -1;

            if (_protocol == ProtocolType.Udp)
                if (other.Protocol == ProtocolType.Tcp)
                    return -1;

            return 0;
        }
    }
}
