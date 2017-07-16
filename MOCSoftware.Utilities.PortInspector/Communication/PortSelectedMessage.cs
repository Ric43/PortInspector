using MOCSoftware.Utilities.PortInspector.Model;
using System.Collections.Generic;

namespace MOCSoftware.Utilities.PortInspector.Communication
{
    internal class PortSelectedMessage : Message
    {
        public PortInfo Port { get; set; }
        public List<PortInfo> SelectedPortList { get; set; }
    }
}
