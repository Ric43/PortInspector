using MOCSoftware.Utilities.PortInspector.Model;
using MOCSoftware.Utilities.PortInspector.Utility;

namespace MOCSoftware.Utilities.PortInspector.Communication
{
    internal class PortAddMessage : Message
    {
        internal PortInfo ExistingPort { get; set; }
        internal PortAddMode Mode
        {
            get { return ExistingPort == null ? PortAddMode.AddMode : PortAddMode.EditMode; }
        }
    }
}
