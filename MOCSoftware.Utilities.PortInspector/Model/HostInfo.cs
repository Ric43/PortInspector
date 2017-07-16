using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using MOCSoftware.Utilities.PortInspector.Properties;

namespace MOCSoftware.Utilities.PortInspector.Model
{
    [Serializable]
    internal sealed class HostInfo : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        private ResolverStates _resolverState = ResolverStates.ResolutionPending;

        public ResolverStates ResolverState
        {
            get
            {
                return _resolverState;
            }
            set
            {
                _resolverState = value;
                OnPropertyChanged("ResolverState");
            }
        }

        public string HostEntry
        {
            set
            {
                _hostName = Resources.HostInfo_ResolvingMessage;
                var task = Task.Factory.StartNew(() => ResolveHost(value));
            }
        }

        private string _hostName;
        public string HostName
        {
            get
            {
                return _hostName;
            }
            set
            {
                _hostName = value;
                OnPropertyChanged("HostName");
                OnPropertyChanged("IpAddress");
            }
        }

        public string IpAddress
        {
            get
            {
                if (_ipAddresses.Count == 0)
                {
                    return string.Empty;
                }
                return _ipAddresses[0].ToString();
            }
        }

        internal IPAddress HostAddress
        {
            get
            {
                return _ipAddresses[0];
            }
        }

        private readonly List<string> _aliases = new List<string>();
        private readonly List<IPAddress> _ipAddresses = new List<IPAddress>();

        internal bool IsValid { get; set; }

        public void AddAliases(string[] aliases)
        {
            foreach (var alias in aliases)
                _aliases.Add(alias);
        }

        public void AddAddresses(IPAddress[] addressList)
        {
            foreach (var ipAddress in addressList)
                _ipAddresses.Add(ipAddress);
        }

        public override string ToString()
        {
            return HostName;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ResolveHost(string hostEntry)
        {
            IsValid = true;

            try
            {
                var ipHostEntry = Dns.GetHostEntry(hostEntry);
                HostName = ipHostEntry.HostName;
                AddAliases(ipHostEntry.Aliases);
                AddAddresses(ipHostEntry.AddressList);

                if (_ipAddresses.Count == 0)
                    throw new Exception("No IP Address returned in name lookup.");

                ResolverState = ResolverStates.ResolutionSucceeded;
            }
            catch (Exception ex)
            {
                IPAddress ipAddress;
                if (IPAddress.TryParse(hostEntry, out ipAddress))
                {
                    HostName = ipAddress.ToString();
                    _ipAddresses.Add(ipAddress);
                    ResolverState = ResolverStates.ResolutionSucceeded;
                }
                else
                {
                    IsValid = false;
                    HostName = string.Format(Resources.HostInfo_ResolutionErrorMessage, hostEntry);
                    ResolverState = ResolverStates.ResolutionFailed;
                }
            }

            App.Mediator.Publish(App._EVENT_HOSTINFO_RESOLVERCOMPLETED);
        }
    }
}
