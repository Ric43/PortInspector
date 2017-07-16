using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MOCSoftware.Utilities.PortInspector.Communication;
using MOCSoftware.Utilities.PortInspector.Properties;
using MOCSoftware.Utilities.PortInspector.Utility;

namespace MOCSoftware.Utilities.PortInspector.ViewModel
{
    internal class AddHostListViewModel : ViewModelBase
    {
        private RelayCommand _cancelCommand;
        public ICommand CancelCommand { get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(param => CloseModal())); } }
        private RelayCommand _addCommand;
        public ICommand AddCommand { get { return _addCommand ?? (_addCommand = new RelayCommand(param => OnAddCommand())); } }

        private List<string> _originalHostNameList; 

        private string _hostNames;
        public string HostNames { get { return _hostNames; } set { _hostNames = value; } }

        private bool _isVisible = false;
        public bool IsVisible { get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }
        public string Title
        {
            get { return Resources.AddHostListView_Title; }
        }

        private ICommand _hostListAddCommand;
        internal ICommand HostListAddCommand
        {
            get { return _hostListAddCommand ?? (_hostListAddCommand = new RelayCommand(e => OpenModal((HostListAddMessage)e))); }
        }

        internal AddHostListViewModel()
        {
            App.Mediator.Subscribe(App._EVENT_HOSTLIST_ADD, this, HostListAddCommand);
        }

        ~AddHostListViewModel()
        {
            App.Mediator.Unsubscribe(App._EVENT_HOSTLIST_ADD, this, HostListAddCommand);
        }

        private void OnAddCommand()
        {
            var hostList = new List<string>(HostNames.ToUpperInvariant().Replace("\r", string.Empty).Split('\n'));
            hostList.RemoveAll(e => e.Trim().Equals(string.Empty));
            var removeList = (from o in _originalHostNameList
                             where !hostList.Contains(o.ToUpperInvariant())
                             select o).ToList();
            var addList = (from h in hostList
                          where !_originalHostNameList.Contains(h.ToUpperInvariant())
                          select h).ToList();

            App.Mediator.Publish(App._EVENT_ADDHOSTLIST_ADDED, new HostListAddedMessage{AddList = addList, RemoveList = removeList});
            CloseModal();
        }

        private void OpenModal(HostListAddMessage message)
        {
            OnPropertyChanged("Title");
            IsVisible = true;
            _originalHostNameList = message.HostList.Select(e => e.ToUpperInvariant()).ToList();
            var hostNameBuilder = new StringBuilder();
            foreach (var hostName in message.HostList)
                hostNameBuilder.AppendFormat("{0}\n", hostName);
            HostNames = hostNameBuilder.ToString();
            OnPropertyChanged("HostNames");
            App.Mediator.Publish(App._EVENT_MODALWINDOWMESSAGE_STATECHANGED, new ModalWindowMessage { WindowState = WindowStates.Open });
        }

        private void CloseModal()
        {
            IsVisible = false;
            App.Mediator.Publish(App._EVENT_MODALWINDOWMESSAGE_STATECHANGED, new ModalWindowMessage { WindowState = WindowStates.Closed });
        }
    }
}
