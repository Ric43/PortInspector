using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MOCSoftware.Utilities.PortInspector.Communication;
using MOCSoftware.Utilities.PortInspector.Creator;
using MOCSoftware.Utilities.PortInspector.Model;
using MOCSoftware.Utilities.PortInspector.Properties;
using MOCSoftware.Utilities.PortInspector.Utility;
using MOCSoftware.Utilities.PortInspector.Persistence;
using System.Windows;
using System.Collections.Specialized;

namespace MOCSoftware.Utilities.PortInspector.ViewModel
{
    internal class HostListViewModel : ViewModelBase, IInitialisableViewModel
    {
        private List<PortInfo> _selectedPorts = new List<PortInfo>();

        private IHostDataPersistor _persistor;
        public IHostDataPersistor Persistor
        {
            get
            {
                return _persistor ?? (_persistor = new HostDataPersistor());
            }

        }

        private bool _abortPending;
        public bool AbortEnabled
        {
            get
            {
                return !_abortPending;
            }
            set
            {
                _abortPending = !value;
                OnPropertyChanged("AbortEnabled");
            }
        }

        private bool _isScanning = false;
        private bool IsScanning
        {
            get
            {
                return _isScanning;
            }
            set
            {
                _isScanning = value;
                AbortEnabled = true;
                OnPropertyChanged("StartButtonVisibility");
                OnPropertyChanged("StopButtonVisibility");
            }
        }

        public Visibility StartButtonVisibility
        {
            get
            {
                if (_isScanning)
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
            set { }
        }

        public Visibility StopButtonVisibility
        {
            get
            {
                if (!_isScanning)
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
            set { }
        }

        private const string _HOST_LIST_CONTEXT_MENU_NAME = "HOSTLISTCONTEXTMENU";
        private const string _HOST_EDIT_DELETE_TAG_NAME = "DELETE";

        internal MainMenuViewModel MainMenu { get; set; }

        private MenuItemCollection _menuItemCollection;
        internal MenuItemCollection HostMenu { get { return _menuItemCollection; } }

        private HostInfoFactory _hostInfoFactory;

        private RelayCommand _addHostListCommand;
        private RelayCommand _removeHostCommand;
        private RelayCommand _saveHostListCommand;
        private RelayCommand _loadHostListCommand;
        private RelayCommand _clearHostListCommand;
        private RelayCommand _initiateScanning;
        private RelayCommand _abortScanning;

        private RelayCommand _hostListAddedCommand;
        internal RelayCommand HostListAddedCommand
        {
            get { return _hostListAddedCommand ?? (_hostListAddedCommand = new RelayCommand(e => OnHostListAdded((HostListAddedMessage)e)));}
        }

        private RelayCommand _hostFileLoadedCommand;
        internal RelayCommand HostFileLoadedCommand
        {
            get { return _hostFileLoadedCommand ?? (_hostFileLoadedCommand = new RelayCommand(e => OnHostListLoaded((HostFileLoadedMessage)e)));}
        }

        private RelayCommand _portSelectedCommand;
        internal RelayCommand PortSelectedCommand
        {
            get { return _portSelectedCommand ?? (_portSelectedCommand = new RelayCommand(e => OnPortSelected((PortSelectedMessage)e)));}
        }

        private RelayCommand _scanCompletedCommand;
        internal RelayCommand ScanCompletedCommand
        {
            get { return _scanCompletedCommand ?? (_scanCompletedCommand = new RelayCommand(e => OnScanningComplete())); }
        }

        private RelayCommand _resolverCompletedCommand;
        internal RelayCommand ResolverCompletedCommand
        {
            get { return _resolverCompletedCommand ?? (_resolverCompletedCommand = new RelayCommand(e => OnResolverCompleted()));}
        }

        public MenuItemCollection MenuItems
        {
            get
            {
                return MainMenu.GetContextMenu(_HOST_LIST_CONTEXT_MENU_NAME);
            }
        }

        public ICommand AddHostListCommand
        {
            get { return _addHostListCommand ?? (_addHostListCommand = new RelayCommand(param => OnAddHostList())); }
        }

        public ICommand RemoveHostCommand
        {
            get { return _removeHostCommand ?? (_removeHostCommand = new RelayCommand(param => OnRemoveHost())); }
        }

        public ICommand SaveHostListCommand
        {
            get { return _saveHostListCommand ?? (_saveHostListCommand = new RelayCommand(param => OnSaveHostList())); }
        }

        public ICommand LoadHostListCommand
        {
            get { return _loadHostListCommand ?? (_loadHostListCommand = new RelayCommand(param => OnLoadHostList())); }
        }

        public ICommand ClearHostListCommand
        {
            get { return _clearHostListCommand ?? (_clearHostListCommand = new RelayCommand(param => OnClearHostList())); }
        }

        public ICommand InitiateScanning
        {
            get { return _initiateScanning ?? (_initiateScanning = new RelayCommand(param => OnInitiateScanning())); }
        }

        public ICommand AbortScanning
        {
            get { return _abortScanning ?? (_abortScanning = new RelayCommand(param => OnAbortScanning())); }
        }

        public bool HostIsSelected
        {
            get
            {
                return SelectedHost != null;
            }
        }

        private HostInfo _selectedHost;
        public HostInfo SelectedHost
        {
            get
            {
                return _selectedHost;
            }
            set
            {
                _selectedHost = value;
                var menuItems = MainMenu.GetMenuItemsByTag(_HOST_EDIT_DELETE_TAG_NAME);
                menuItems.ForEach(e => e.IsEnabled = _selectedHost != null);
                OnPropertyChanged("SelectedHost");
                OnPropertyChanged("HostIsSelected");
            }
        }

        private readonly HostInfoList _hostList = new HostInfoList(); 
        public HostInfoList HostList
        {
            get { return _hostList; }
        }

        internal HostInfoFactory HostInfoFactory
        {
            get { return _hostInfoFactory ?? (_hostInfoFactory = new HostInfoFactory()); }
        }

        public bool AllHostsValid
        {
            get
            {
                var inValidHost = _hostList.FirstOrDefault(host => !host.IsValid);
                var allHostsValid = (inValidHost == null && _hostList.Count > 0 && _selectedPorts.Count > 0);
                return allHostsValid;
            }
        }

        private string _hostName = string.Empty;
        public string HostName
        {
            get { return _hostName; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var hostInfo = HostInfoFactory.CreateHostInfo(value);
                    _hostList.Add(hostInfo);
                    OnPropertyChanged("HostName");
                    OnPropertyChanged("AllHostsValid");
                }
            }
        }

        public HostListViewModel()
        {
            App.Mediator.Subscribe(App._EVENT_ADDHOSTLIST_ADDED, this, HostListAddedCommand);
            App.Mediator.Subscribe(App._EVENT_HOSTDATA_LOADED, this, HostFileLoadedCommand);
            App.Mediator.Subscribe(App._EVENT_PORT_PORTSELECTED, this, PortSelectedCommand);
            App.Mediator.Subscribe(App._EVENT_SCANNER_COMPLETE, this, ScanCompletedCommand);
            App.Mediator.Subscribe(App._EVENT_HOSTINFO_RESOLVERCOMPLETED, this, ResolverCompletedCommand);
        }

        ~HostListViewModel()
        {
            App.Mediator.Unsubscribe(App._EVENT_ADDHOSTLIST_ADDED, this, HostListAddedCommand);
            App.Mediator.Unsubscribe(App._EVENT_HOSTDATA_LOADED, this, HostFileLoadedCommand);
            App.Mediator.Unsubscribe(App._EVENT_PORT_PORTSELECTED, this, PortSelectedCommand);
            App.Mediator.Unsubscribe(App._EVENT_SCANNER_COMPLETE, this, ScanCompletedCommand);
            App.Mediator.Unsubscribe(App._EVENT_HOSTINFO_RESOLVERCOMPLETED, this, ResolverCompletedCommand);
        }

        public void Initialise()
        {
            InitViewModelMenu();
            MainMenu.MergeMenus(_menuItemCollection);
            _hostList.CollectionChanged += OnHostListChanged;
        }

        private void InitViewModelMenu()
        {
            _menuItemCollection = new MenuItemCollection
                                      {
                                          new MenuItemViewModel { Header = "_File"},
                                          new MenuItemViewModel { Header = "S_ave Host List", Parent = "_File", Command = SaveHostListCommand, Priority = 11},
                                          new MenuItemViewModel { Header = "L_oad Host List", Parent = "_File", Command = LoadHostListCommand, Priority = 11},
                                          new MenuItemViewModel {Header = "_Host"},
                                          new MenuItemViewModel
                                              {Header = "_Add Host List", Parent = "_Host", Command = AddHostListCommand, ContextMenuName = _HOST_LIST_CONTEXT_MENU_NAME},
                                          new MenuItemViewModel
                                              {Header = "_Delete Host", Parent = "_Host", Command = RemoveHostCommand, ContextMenuName = _HOST_LIST_CONTEXT_MENU_NAME,
                                              IsEnabled = false, Tag=_HOST_EDIT_DELETE_TAG_NAME},
                                          new MenuItemViewModel
                                              {Header = "_Clear Host List", Parent = "_Host", Command = ClearHostListCommand, ContextMenuName = _HOST_LIST_CONTEXT_MENU_NAME}
                                      };
        }

        private void OnAddHostList()
        {
            var hostList = HostList.Select(entry => entry.HostName).ToList();
            App.Mediator.Publish(App._EVENT_HOSTLIST_ADD, new HostListAddMessage{HostList = hostList});
        }

        private void OnHostListAdded(HostListAddedMessage message)
        {
            message.AddList.ForEach(e => _hostList.Add(HostInfoFactory.CreateHostInfo(e)));
            message.RemoveList.ForEach(e => _hostList.RemoveHostByHostName(e));
            OnPropertyChanged("AllHostsValid");
        }

        private void OnPortSelected(PortSelectedMessage message)
        {
            if (message.SelectedPortList == null)
            {
                if (message.Port.IsSelected)
                {
                    if (!_selectedPorts.Contains(message.Port))
                        _selectedPorts.Add(message.Port);
                }
                else
                {
                    if (_selectedPorts.Contains(message.Port))
                        _selectedPorts.Remove(message.Port);
                }
            }
            else
            {
                _selectedPorts.Clear();
                foreach (var port in message.SelectedPortList)
                    _selectedPorts.Add(port);
            }
            OnPropertyChanged("AllHostsValid");
        }

        private void OnRemoveHost()
        {
            HostList.Remove(SelectedHost);
            SelectedHost = null;
        }

        private void OnSaveHostList()
        {
            Persistor.SaveHostData(_hostList.Select(e => e.HostName).ToList());
        }

        private void OnLoadHostList()
        {
            Persistor.LoadHostData();
        }

        private void OnClearHostList()
        {
            _hostList.Clear();
        }

        private void OnInitiateScanning()
        {
            IsScanning = true;
            App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage { ShowBusy = false, StatusMessage = Resources.HostListViewModel_ScanningMessage });
            App.Mediator.Publish(App._EVENT_HOSTLIST_SCANSTART, new ScanInitiatedMessage
            {
                HostsToScan = _hostList.ToList(),
                PortsToScan = _selectedPorts
            });
        }

        private void OnAbortScanning()
        {
            AbortEnabled = false;
            App.Mediator.Publish(App._EVENT_HOSTLIST_SCANABORT);
        }

        private void OnScanningComplete()
        {
            IsScanning = false;
            AbortEnabled = true;
            App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage { ShowBusy = false, StatusMessage = string.Empty, ProgressValue = App._PROGRESS_BAR_HIDE});
        }

        private void OnHostListLoaded(HostFileLoadedMessage message)
        {
            Application.Current.Dispatcher.Invoke((Action)(() => _hostList.Clear()));
            message.HostData.ForEach(e => AddPersistedHost(e));
        }

        private void AddPersistedHost(string hostName)
        {
            var host = HostInfoFactory.CreateHostInfo(hostName);
            Application.Current.Dispatcher.Invoke((Action)(() => _hostList.Add(host)));
        }

        private void OnHostListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("AllHostsValid");
        }

        private void OnResolverCompleted()
        {
            OnPropertyChanged("AllHostsValid");
        }
    }
}
