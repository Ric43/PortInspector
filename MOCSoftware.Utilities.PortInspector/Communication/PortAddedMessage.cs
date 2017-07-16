using MOCSoftware.Utilities.PortInspector.Model;
using MOCSoftware.Utilities.PortInspector.Utility;

namespace MOCSoftware.Utilities.PortInspector.Communication
{
    internal class PortAddedMessage : Message
    {
        internal bool IsCancelled
        {
            get { return NewPort == null; }
        }

        internal PortAddMode Mode { get; set; }
        internal PortInfo NewPort { get; set; }
    }
}
