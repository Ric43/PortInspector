using System.Collections.Generic;

namespace MOCSoftware.Utilities.PortInspector.Communication
{
    internal class HostListAddedMessage : Message
    {
        public List<string> AddList { get; set; }
        public List<string> RemoveList { get; set; }
    }
}
