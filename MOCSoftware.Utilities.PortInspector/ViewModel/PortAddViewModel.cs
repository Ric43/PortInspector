using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using MOCSoftware.Utilities.PortInspector.Communication;
using MOCSoftware.Utilities.PortInspector.Model;
using MOCSoftware.Utilities.PortInspector.Properties;
using MOCSoftware.Utilities.PortInspector.Utility;

namespace MOCSoftware.Utilities.PortInspector.ViewModel
{
    internal class PortAddViewModel : ViewModelBase
    {
        private RelayCommand _portAddedCommand;
        private RelayCommand _cancelCommand;

        private string _portName;
        private short _portNumber;
        private string _protocol;
        private string _alias;
        private string _comment;
        private PortAddMode _mode;
        private bool _isVisible;

        public string Title
        {
            get
            {
                switch (_mode)
                {
                    case PortAddMode.AddMode:
                        return Resources.AddPortWindowTitle;
                    default:
                        return Resources.EditPortWindowTitle;
                }
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

        public bool IsWindowInAddMode
        {
            get { return _mode == PortAddMode.AddMode; } 
        }

        public bool IsWindowInEditMode
        {
            get { return _mode == PortAddMode.EditMode; }
        }

        public string PortName
        {
            get { return _portName ?? string.Empty; }
            set
            {
                _portName = value;
                OnPropertyChanged("PortName");
            }
        }

        public string PortNumber
        {
            get { return _portNumber > 0 ? _portNumber.ToString(CultureInfo.InvariantCulture) : string.Empty; }
            set
            {
                short.TryParse(value, out _portNumber);
                OnPropertyChanged("PortNumber");
            }
        }

        public string Protocol
        {
            get{ return _protocol ?? "All";}
            set
            {
                _protocol = value;
                OnPropertyChanged("Protocol");
            }
        }

        public string Alias
        {
            get { return _alias ?? string.Empty; }
            set
            {
                _alias = value;
                OnPropertyChanged("Alias");
            }
        }

        public string Comment
        {
            get { return _comment ?? string.Empty; }
            set
            {
                _comment = value;
                OnPropertyChanged("Comment");
            }
        }

        public List<string> ProtocolList
        {
            get
            {
                var protocolList = new List<string>();
                protocolList.AddRange(Enum.GetNames(typeof(PortInspectorProtocols)).ToList());
                return protocolList;
            }
        }

        public ICommand PortAddedCommand
        {
            get { return _portAddedCommand ?? (_portAddedCommand = new RelayCommand(param => OnRequestPortCommand())); }
        }

        public ICommand CancelledCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(param => OnCancelled())); }
        }

        private ICommand _openCommand;
        internal ICommand OpenCommand
        {
            get { return _openCommand ?? (_openCommand = new RelayCommand(e => OpenModal((PortAddMessage)e))); }
        }

        internal PortAddViewModel()
        {
            App.Mediator.Subscribe(App._EVENT_PORTLIST_ADD, this, OpenCommand);
        }

        ~PortAddViewModel()
        {
            App.Mediator.Unsubscribe(App._EVENT_PORTLIST_ADD, this, OpenCommand);
        }

        private void OnCancelled()
        {
            CloseModal();
        }

        private void OnRequestPortCommand()
        {
            App.Mediator.Publish(App._EVENT_PORTADD_ADDED, new PortAddedMessage
                              {
                                  Mode = _mode,
                                  NewPort =
                                      new PortInfo(PortName, Protocol, short.Parse(PortNumber), Alias, Comment, false)
                              });
            CloseModal();
        }

        private void OpenModal(PortAddMessage message)
        {
            OnPropertyChanged("Title");
            IsVisible = true;
            _mode = message.Mode;
            SetInitialValues(message.ExistingPort);
            App.Mediator.Publish(App._EVENT_MODALWINDOWMESSAGE_STATECHANGED, new ModalWindowMessage{WindowState = WindowStates.Open});
        }

        private void SetInitialValues(PortInfo initialInfo)
        {
            if (initialInfo != null)
            {
                Alias = initialInfo.Alias;
                Comment = initialInfo.Description;
                PortName = initialInfo.PortName;
                PortNumber = initialInfo.PortNumber.ToString(CultureInfo.InvariantCulture);
                Protocol = initialInfo.Protocol.ToString().ToUpperInvariant();
            }
            else
            {
                Alias = string.Empty;
                Comment = string.Empty;
                PortName = string.Empty;
                PortNumber = string.Empty;
                Protocol = string.Empty;
            }
        }

        private void CloseModal()
        {
            IsVisible = false;
            App.Mediator.Publish(App._EVENT_MODALWINDOWMESSAGE_STATECHANGED, new ModalWindowMessage { WindowState = WindowStates.Closed });
        }
    }
}
