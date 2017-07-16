using System.Windows.Input;
using MOCSoftware.Utilities.PortInspector.ViewModel;

namespace MOCSoftware.Utilities.PortInspector.Communication
{
    internal interface IMediator
    {
        void Subscribe(string eventName, ViewModelBase source, ICommand action);
        void Unsubscribe(string eventName, ViewModelBase source, ICommand action);
        void Publish(string eventName);
        void Publish(string eventName, Message eventArguments);
    }
}
