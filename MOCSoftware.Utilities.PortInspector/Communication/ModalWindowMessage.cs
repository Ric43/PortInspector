using MOCSoftware.Utilities.PortInspector.Utility;

namespace MOCSoftware.Utilities.PortInspector.Communication
{
    internal class ModalWindowMessage : Message
    {
        internal WindowStates WindowState { get; set; }
    }
}
