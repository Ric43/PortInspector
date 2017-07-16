using MOCSoftware.Utilities.PortInspector.Model;
using System.Collections.Generic;

namespace MOCSoftware.Utilities.PortInspector.Communication
{
    internal class ScanInitiatedMessage : Message
    {
        internal List<PortInfo> PortsToScan { get; set; }
        internal List<HostInfo> HostsToScan { get; set; }
    }
}
