using System;
using System.Windows.Input;
using MOCSoftware.Utilities.PortInspector.ViewModel;

namespace MOCSoftware.Utilities.PortInspector.Communication
{
    internal class EventPayload : IEquatable<EventPayload>   
    {
        internal ViewModelBase EventSource { get; set; }
        internal ICommand EventAction { get; set; }

        public bool Equals(EventPayload other)
        {
            return EventAction.Equals(other.EventAction);
        }
    }
}
