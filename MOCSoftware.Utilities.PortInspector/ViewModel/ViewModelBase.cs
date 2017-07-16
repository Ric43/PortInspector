using System;
using System.ComponentModel;
using MOCSoftware.Utilities.PortInspector.Communication;

namespace MOCSoftware.Utilities.PortInspector.ViewModel
{
    internal abstract class ViewModelBase : INotifyPropertyChanged, IDisposable, ICommunicator
    {
        public virtual string DisplayName { get; protected set; }
        public event PropertyChangedEventHandler PropertyChanged;
       
        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            var handler = PropertyChanged;
            if (handler != null)
            {
                var eventArgs = new PropertyChangedEventArgs(propertyName);
                handler(this, eventArgs);
            }
        }

        public void Dispose()
        {
        }

        public void VerifyPropertyName(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return;

            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                throw new Exception("Invalid property name: " + propertyName);
            }
        }

        protected void OnViewCommand(EventHandler handler)
        {
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public bool Equals(ICommunicator other)
        {
            return GetType().ToString().Equals(other.GetType().ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
