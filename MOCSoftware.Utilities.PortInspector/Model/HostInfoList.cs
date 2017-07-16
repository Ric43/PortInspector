using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MOCSoftware.Utilities.PortInspector.Model
{
    internal sealed class HostInfoList : ObservableCollection<HostInfo>
    {
        internal void RemoveHostByHostName(string hostName)
        {
            Remove(this.FirstOrDefault(e => e.HostName.Equals(hostName, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
