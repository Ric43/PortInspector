using System.Collections.Generic;
using System.Windows.Input;
using MOCSoftware.Utilities.PortInspector.ViewModel;

namespace MOCSoftware.Utilities.PortInspector.Communication
{
    internal class Mediator : IMediator
    {
        private readonly Dictionary<string, List<EventPayload>> _eventRegistry = new Dictionary<string, List<EventPayload>>();

        public void Subscribe(string eventName, ViewModelBase source, ICommand action)
        {
            lock (_eventRegistry)
            {
                var key = GetKey(eventName);
                var payload = new EventPayload {EventSource = source, EventAction = action};
                if (!_eventRegistry.ContainsKey(key))
                    _eventRegistry.Add(key, new List<EventPayload>());
                if (!_eventRegistry[key].Contains(payload))
                    _eventRegistry[key].Add(payload);
            }
        }

        public void Unsubscribe(string eventName, ViewModelBase source, ICommand action)
        {
            var key = GetKey(eventName);
            var payload = new EventPayload {EventSource = source, EventAction = action};
            lock (_eventRegistry)
            {
                if (_eventRegistry[key].Contains(payload))
                    _eventRegistry[key].Remove(payload);
            }
        }

        public void Publish(string eventName)
        {
            Publish(eventName, null);
        }

        public void Publish(string eventName, Message eventArguments)
        {
            var key = GetKey(eventName);
            foreach (var payload in _eventRegistry[key])
            {
                if (payload.EventSource != null && payload.EventAction != null)
                    payload.EventAction.Execute(eventArguments);
            }
        }

        private string GetKey(string eventName)
        {
            return eventName.Trim().ToUpperInvariant();
        }
    }
}
