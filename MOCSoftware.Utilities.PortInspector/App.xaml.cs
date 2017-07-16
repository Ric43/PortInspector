using System;
using System.Windows;
using MOCSoftware.Utilities.PortInspector.Communication;
using MOCSoftware.Utilities.PortInspector.ViewModel;

namespace MOCSoftware.Utilities.PortInspector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly IMediator _mediator = new Mediator();

        internal const string _EVENT_ADDHOSTLIST_ADDED = "ADDHOSTLIST.ADDED";
        internal const string _EVENT_ERROR = "ERROR";
        internal const string _EVENT_HOSTDATA_LOADED = "HOSTDATA.LOADED";
        internal const string _EVENT_HOSTINFO_RESOLVERCOMPLETED = "HOSTINFO.RESOLVERCOMPLETED";
        internal const string _EVENT_HOSTLIST_ADD = "HOSTLIST.ADD";
        internal const string _EVENT_HOSTLIST_SCANABORT = "HOSTLIST.SCANABORT";
        internal const string _EVENT_HOSTLIST_SCANSTART = "HOSTLIST.SCANSTART";
        internal const string _EVENT_MAINWINDOW_ABOUT = "MAINWINDOW.ABOUT";
        internal const string _EVENT_MODALWINDOWMESSAGE_STATECHANGED = "MODALWINDOWMESSAGE.STATECHANGED";
        internal const string _EVENT_PORT_PORTSELECTED = "PORT.PORTSELECTED";
        internal const string _EVENT_PORTADD_ADDED = "PORTADD.ADDED";
        internal const string _EVENT_PORTLIST_ADD = "PORTLIST.ADD";
        internal const string _EVENT_SCANNER_COMPLETE = "SCANNER.COMPLETE";
        internal const string _EVENT_STATUSUPDATE = "STATUSUPDATE";

        internal const int _PROGRESS_BAR_HIDE = -1;

        internal static IMediator Mediator
        {
            get { return _mediator; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var portListViewModel = new PortListViewModel();
            var hostListViewModel = new HostListViewModel();
            var scannerResultsViewModel = new ScannerResultsViewModel();

            var window = new MainWindow();

            var viewModel = new MainWindowViewModel();
            viewModel.PortDetail = portListViewModel;
            viewModel.HostDetail = hostListViewModel;
            viewModel.ScannerResults = scannerResultsViewModel;
            viewModel.Initialise();

            EventHandler OnRequestClose = null;
            OnRequestClose = delegate
            {
                viewModel.RequestClose -= OnRequestClose;
                window.Close();
            };

            viewModel.RequestClose += OnRequestClose;
            window.Loaded += viewModel.OnWindowLoaded;

            window.DataContext = viewModel;

            window.Show();
        }
    }
}
