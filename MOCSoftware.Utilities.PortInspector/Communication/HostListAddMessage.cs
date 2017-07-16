using System.Collections.Generic;

namespace MOCSoftware.Utilities.PortInspector.Communication
{
    internal class HostListAddMessage : Message
    {
        public List<string> HostList { get; set; } 
    }
}
