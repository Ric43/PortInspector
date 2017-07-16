using System.Collections.Generic;

namespace MOCSoftware.Utilities.PortInspector.Communication
{
    internal class HostFileLoadedMessage : Message
    {
        public List<string> HostData { get; set; }
    }
}
