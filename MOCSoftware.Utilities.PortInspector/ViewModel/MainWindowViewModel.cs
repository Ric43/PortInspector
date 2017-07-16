using System;
using System.Windows;
using System.Windows.Input;
using MOCSoftware.Utilities.PortInspector.Communication;
using MOCSoftware.Utilities.PortInspector.Persistence;
using MOCSoftware.Utilities.PortInspector.Utility;

namespace MOCSoftware.Utilities.PortInspector.ViewModel
{
    internal sealed class MainWindowViewModel : ViewModelBase, IInitialisableViewModel
    {
        private const int _PROGRESS_BAR_MINIMUM = 0;
        private const int _PROGRESS_BAR_MAXIMUM = 100;

        public PortListViewModel PortDetail { get; set; }
        public HostListViewModel HostDetail { get; set; }
        public ScannerResultsViewModel ScannerResults { get; set; }

        private MainMenuViewModel _mainMenu;
        public MainMenuViewModel MainMenu
        {
            get { return _mainMenu ?? (_mainMenu = new MainMenuViewModel()); }
            set
            {
                _mainMenu = value;
                OnPropertyChanged("MainMenu");
            }
        }

        private PortAddViewModel _portAddWindow;
        public PortAddViewModel PortAddWindow
        {
            get { return _portAddWindow ?? (_portAddWindow = new PortAddViewModel()); }
            set
            {
                _portAddWindow = value;
                OnPropertyChanged("PortAddWindow");
            }
        }

        private AddHostListViewModel _addHostListWindow;
        public AddHostListViewModel AddHostListWindow
        {
            get { return _addHostListWindow ?? (_addHostListWindow = new AddHostListViewModel()); }
            set
            {
                _addHostListWindow = value;
                OnPropertyChanged("AddHostListWindow");
            }
        }

        private AboutViewModel _aboutViewModel;
        public AboutViewModel AboutViewModel
        {
            get { return _aboutViewModel ?? (_aboutViewModel = new AboutViewModel()); }
            set
            {
                _aboutViewModel = value;
                OnPropertyChanged("AboutViewModel");
            }
        }

        private ModalMessageDialogViewModel _messageDialog;
        public ModalMessageDialogViewModel MessageDialog
        {
            get { return _messageDialog ?? (_messageDialog = new ModalMessageDialogViewModel()); }
            set
            {
                _messageDialog = value;
                OnPropertyChanged("MessageDialog");
            }
        }

        private MenuItemCollection _menuItemCollection;

        public event EventHandler RequestClose;
        private delegate void RefreshObserverDelegate();

        private RelayCommand _closeCommand;
        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(param => OnViewCommand(RequestClose))); }
        }

        private RelayCommand _aboutCommand;
        public ICommand AboutCommand
        {
            get { return _aboutCommand ?? (_aboutCommand = new RelayCommand(param => OnAboutCommand())); }
        }

        #region ViewModel Data Fields

        private string _statusMessage;
        private bool _formIsEnabled;
        private Cursor _formCursor;
        private Visibility _progressBarVisibility;
        private int _progressBarMinimum;
        private int _progressBarMaximum;
        private int _progressBarCurrent;

        #endregion

        #region ViewModel Binding Properties

        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = value;
                OnPropertyChanged("StatusMessage");
            }
        }

        public bool FormIsEnabled
        {
            get
            {
                return _formIsEnabled;
            }
            set
            {
                _formIsEnabled = value;
                OnPropertyChanged("FormIsEnabled");
            }
        }

        public Cursor FormCursor
        {
            get
            {
                return _formCursor;
            }
            set
            {
                _formCursor = value;
                OnPropertyChanged("FormCursor");
            }
        }

        public Visibility ProgressBarVisibility
        {
            get
            {
                return _progressBarVisibility;
            }
            set
            {
                _progressBarVisibility = value;
                OnPropertyChanged("ProgressBarVisibility");
            }
        }

        public int ProgressBarMinimum
        {
            get
            {
                return _progressBarMinimum;
            }
            set
            {
                _progressBarMinimum = value;
                OnPropertyChanged("ProgressBarMinimum");
            }
        }

        public int ProgressBarMaximum
        {
            get
            {
                return _progressBarMaximum;
            }
            set
            {
                _progressBarMaximum = value;
                OnPropertyChanged("ProgressBarMaximum");
            }
        }

        public int ProgressBarCurrent
        {
            get
            {
                return _progressBarCurrent;
            }
            set
            {
                _progressBarCurrent = value;
                OnPropertyChanged("ProgressBarCurrent");
            }
        }

        #endregion

        private ICommand _statusUpdateCommand;
        internal ICommand StatusUpdateCommand
        {
            get { return _statusUpdateCommand ?? (_statusUpdateCommand = new RelayCommand(e => StatusUpdate((StatusUpdateMessage)e))); }
        }

        private ICommand _windowStateChangedCommand;
        internal ICommand WindowStateChangedCommand
        {
            get { return _windowStateChangedCommand ?? (_windowStateChangedCommand = new RelayCommand(e => ModalDialogStateChanged((ModalWindowMessage)e))); }
        }

        public MainWindowViewModel()
        {
            App.Mediator.Subscribe(App._EVENT_STATUSUPDATE, this, StatusUpdateCommand);
            App.Mediator.Subscribe(App._EVENT_MODALWINDOWMESSAGE_STATECHANGED, this, WindowStateChangedCommand);
        }

        ~MainWindowViewModel()
        {
            App.Mediator.Unsubscribe(App._EVENT_STATUSUPDATE, this, StatusUpdateCommand);
            App.Mediator.Unsubscribe(App._EVENT_MODALWINDOWMESSAGE_STATECHANGED, this, WindowStateChangedCommand);
        }

        public void Initialise()
        {
            StatusMessage = string.Empty;
            FormIsEnabled = false;
            FormCursor = Cursors.AppStarting;
            InitProgress();

            InitViewModelMenu();
            MainMenu.MergeMenus(_menuItemCollection);

            PortDetail.MainMenu = MainMenu;
            PortDetail.Initialise();
            HostDetail.MainMenu = MainMenu;
            HostDetail.Initialise();
            ScannerResults.MainMenu = MainMenu;
            ScannerResults.Initialise();
        }

        private void InitViewModelMenu()
        {
            _menuItemCollection = new MenuItemCollection
                                      {
                                          new MenuItemViewModel {Header = "_File"},
                                          new MenuItemViewModel
                                              {Header = "E_xit", Parent = "_File", Command = CloseCommand, Priority = 100},
                                          new MenuItemViewModel {Header = "_Help", Priority = 10},
                                          new MenuItemViewModel
                                              {Header = "_About", Parent = "_Help", Command = AboutCommand, Priority  = 10}
                                      };
        }

        public void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            PortDetail.LoadPortData(LoadFromSources.TryDataFileFirstThenServicesFile);
        }

        public void StatusUpdate(StatusUpdateMessage message)
        {
            if (message.ProgressValue > App._PROGRESS_BAR_HIDE)
                UpdateProgress(message.ProgressValue);
            else
                ProgressBarVisibility = Visibility.Hidden;

            if (message.StatusMessage != null)
                StatusMessage = message.StatusMessage;

            ShowBusy(message.ShowBusy);
        }

        public void InitProgress()
        {
            ProgressBarMinimum = _PROGRESS_BAR_MINIMUM;
            ProgressBarMaximum = _PROGRESS_BAR_MAXIMUM;
            ProgressBarCurrent = _PROGRESS_BAR_MINIMUM;
            ProgressBarVisibility = Visibility.Hidden;
        }

        public void UpdateProgress(int value)
        {
            ProgressBarVisibility = Visibility.Visible;

            ProgressBarCurrent = value;
        }

        public void ShowBusy(bool busy)
        {
            FormCursor = busy ? Cursors.Wait : Cursors.Arrow;
            FormIsEnabled = !busy;
        }

        private void ModalDialogStateChanged(ModalWindowMessage message)
        {
            FormIsEnabled = message.WindowState == WindowStates.Closed;
        }

        private void OnAboutCommand()
        {
            App.Mediator.Publish(App._EVENT_MAINWINDOW_ABOUT);
        }
    }
}
