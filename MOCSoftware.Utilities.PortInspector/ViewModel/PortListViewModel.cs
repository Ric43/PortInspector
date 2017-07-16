using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MOCSoftware.Utilities.PortInspector.Communication;
using MOCSoftware.Utilities.PortInspector.Model;
using MOCSoftware.Utilities.PortInspector.Persistence;
using MOCSoftware.Utilities.PortInspector.Properties;
using MOCSoftware.Utilities.PortInspector.Utility;

namespace MOCSoftware.Utilities.PortInspector.ViewModel
{
    internal class PortListViewModel : ViewModelBase, IInitialisableViewModel
    {
        private const string _PORT_LIST_CONTEXT_MENU_NAME = "PORTLISTCONTEXTMENU";
        private const string _PORT_EDIT_DELETE_TAG_NAME = "EDITDELETE";

        internal MainMenuViewModel MainMenu { get; set; }

        private MenuItemCollection _menuItemCollection;
        internal MenuItemCollection PortMenu { get { return _menuItemCollection; } }

        private Ports _ports;

        private RelayCommand _addPortCommand;
        private RelayCommand _clearPortListCommand;
        private RelayCommand _reloadPortListFromServicesFile;
        private RelayCommand _savePortListCommand;
        private RelayCommand _loadPortListCommand;
        private RelayCommand _deletePortCommand;
        private RelayCommand _editPortCommand;

        public ICommand ClearPortListCommand
        {
            get { return _clearPortListCommand ?? (_clearPortListCommand = new RelayCommand(param => OnClearPortList())); }
        }

        public ICommand AddPortCommand
        {
            get { return _addPortCommand ?? (_addPortCommand = new RelayCommand(param => OnAddPort())); }
        }

        public ICommand ReloadPortListFromServicesFileCommand
        {
            get { return _reloadPortListFromServicesFile ?? (_reloadPortListFromServicesFile = new RelayCommand(param => OnReloadPortListFromServicesFile())); }
        }

        public ICommand DeletePortCommand
        {
            get { return _deletePortCommand ?? (_deletePortCommand = new RelayCommand(param => OnDeletePort())); }
        }

        public ICommand EditPortCommand
        {
            get { return _editPortCommand ?? (_editPortCommand = new RelayCommand(param => OnEditPort())); }
        }

        public ICommand SavePortListCommand
        {
            get { return _savePortListCommand ?? (_savePortListCommand = new RelayCommand(param => OnPortListSave())); }
        }

        public ICommand LoadPortListCommand
        {
            get { return _loadPortListCommand ?? (_loadPortListCommand = new RelayCommand(param => OnPortListLoad())); }
        }

        #region ViewModel Data Fields

        private readonly List<string> _protocolList;
        private PortInspectorProtocols _selectedProtocol;
        private string _filterText;
        private PortInfo _selectedPort;
        private PortInfo _savedSelection;

        #endregion

        #region ViewModel Binding Properties

        public Ports PortList
        {
            get { return _ports; }
            set
            {
                _ports = value;
                OnPropertyChanged("PortList");
            }
        }

        public List<string> ProtocolList
        {
            get { return _protocolList; }
        }

        public string SelectedProtocol
        {
            get { return _selectedProtocol.ToString(); }
            set
            {
                if (!Enum.TryParse(value, true, out _selectedProtocol))
                    _selectedProtocol = PortInspectorProtocols.All;

                _ports.SetFilter(_filterText, _selectedProtocol);
            }
        }

        public string PortFilter
        {
            get { return _filterText; }
            set
            {
                _filterText = value;
                _ports.SetFilter(_filterText, _selectedProtocol);
            }
        }

        public PortInfo SelectedPort
        {
            get { return _selectedPort; }
            set
            {
                _selectedPort = value;
                var menuItems = MainMenu.GetMenuItemsByTag(_PORT_EDIT_DELETE_TAG_NAME);
                menuItems.ForEach(e => e.IsEnabled = _selectedPort != null);
            }
        }

        public MenuItemCollection MenuItems
        {
            get
            {
                return MainMenu.GetContextMenu(_PORT_LIST_CONTEXT_MENU_NAME);
            }
        }

        #endregion

        private ICommand _portAddedCommand;
        internal ICommand PortAddedCommand
        {
            get { return _portAddedCommand ?? (_portAddedCommand = new RelayCommand(e => OnPortAdded((PortAddedMessage)e))); }
        }

        public PortListViewModel()
        {
            _ports = new Ports();
            _protocolList = new List<string>();
            _protocolList.AddRange(Enum.GetNames(typeof(PortInspectorProtocols)).ToList());

            App.Mediator.Subscribe(App._EVENT_PORTADD_ADDED, this, PortAddedCommand);
        }

        ~PortListViewModel()
        {
            App.Mediator.Unsubscribe(App._EVENT_PORTADD_ADDED, this, PortAddedCommand);
        }

        public void Initialise()
        {
            InitViewModelMenu();
            MainMenu.MergeMenus(_menuItemCollection);
        }

        private void InitViewModelMenu()
        {
            _menuItemCollection = new MenuItemCollection
                                      {
                                          new MenuItemViewModel { Header = "_File"},
                                          new MenuItemViewModel { Header = "_Save Port List", Parent = "_File", Command = SavePortListCommand, Priority = 10},
                                          new MenuItemViewModel { Header = "_Load Port List", Parent = "_File", Command = LoadPortListCommand, Priority = 10},
                                          new MenuItemViewModel {Header = "_Port"},
                                          new MenuItemViewModel
                                              {Header = "_Add Port", Parent = "_Port", Command = AddPortCommand, ContextMenuName = _PORT_LIST_CONTEXT_MENU_NAME},
                                          new MenuItemViewModel
                                              {Header = "_Clear Port List", Parent = "_Port", Command = ClearPortListCommand, ContextMenuName = _PORT_LIST_CONTEXT_MENU_NAME},
                                          new MenuItemViewModel
                                              {Header = "_Reload Port List from Services File", Parent = "_Port", Command = ReloadPortListFromServicesFileCommand,
                                              ContextMenuName = _PORT_LIST_CONTEXT_MENU_NAME},
                                          new MenuItemViewModel
                                              {Header = "_Delete Port", Parent = "_Port", Command = DeletePortCommand, ContextMenuName = _PORT_LIST_CONTEXT_MENU_NAME,
                                              IsEnabled = false, Tag=_PORT_EDIT_DELETE_TAG_NAME},
                                          new MenuItemViewModel
                                              {Header = "_Edit Port", Parent = "_Port", Command = EditPortCommand, ContextMenuName = _PORT_LIST_CONTEXT_MENU_NAME,
                                              IsEnabled = false, Tag=_PORT_EDIT_DELETE_TAG_NAME}
                                      };
        }

        private void OnAddPort()
        {
            App.Mediator.Publish(App._EVENT_PORTLIST_ADD, new PortAddMessage());
        }

        private void OnEditPort()
        {
            if (SelectedPort != null)
            {
                _savedSelection = new PortInfo(_selectedPort.PortName, _selectedPort.Protocol.ToString(), _selectedPort.PortNumber,
                    _selectedPort.Alias, _selectedPort.Description, _selectedPort.IsSelected);

                App.Mediator.Publish(App._EVENT_PORTLIST_ADD, new PortAddMessage
                                        {
                                            ExistingPort = new PortInfo(
                                                                            SelectedPort.PortName,
                                                                            SelectedPort.Protocol.ToString(),
                                                                            SelectedPort.PortNumber,
                                                                            SelectedPort.Alias,
                                                                            SelectedPort.Description,
                                                                            SelectedPort.IsSelected
                                                                       )
                                        });
            }
        }

        private void OnDeletePort()
        {
            if (SelectedPort != null)
            {
                PortList.Remove(SelectedPort);
            }
        }

        private void OnClearPortList()
        {
            _ports.ClearList();
        }

        private void OnReloadPortListFromServicesFile()
        {
            _ports.ClearList();
            LoadPortData(LoadFromSources.ServicesFile);
        }

        private void OnPortListSave()
        {
            App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage{ ShowBusy = true, StatusMessage = Resources.StatusMessageSavingData});
            _ports.SaveDataAsync(FinishSave);
        }

        private void OnPortListLoad()
        {
            _ports.ClearList();
            App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage{ ShowBusy = true, StatusMessage = Resources.StatusMessageLoadingData });
            LoadPortData(LoadFromSources.DataFile);
        }

        private void FinishSave(IAsyncResult result)
        {
            App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage{ ShowBusy = false, StatusMessage = string.Empty });
        }

        public void LoadPortData(LoadFromSources loadFrom)
        {
            App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage{ ShowBusy = true, StatusMessage = Resources.StatusMessageLoadingData });
            _ports.LoadDataAsync(DataLoadFinished, loadFrom);
        }

        private void DataLoadFinished(IAsyncResult result)
        {
            App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage { ShowBusy = false, StatusMessage = string.Empty });
            OnPropertyChanged("PortList");
        }

        private void OnPortAdded(PortAddedMessage message)
        {
            if (message.Mode == PortAddMode.AddMode)
                _ports.AddPort(message.NewPort);
            else
                _ports.EditPort(_savedSelection, message.NewPort);
        }
    }
}
                                                     