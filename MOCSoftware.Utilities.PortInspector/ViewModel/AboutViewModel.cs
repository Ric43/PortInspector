using MOCSoftware.Utilities.PortInspector.Utility;
using System.Reflection;

namespace MOCSoftware.Utilities.PortInspector.ViewModel
{
    internal class AboutViewModel : ViewModelBase
    {
        private bool _isVisible = false;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

        private string _version;
        public string Version
        {
            get
            {
                return _version ?? string.Empty;
            }
            set
            {
                _version = value;
                OnPropertyChanged("Version");
            }
        }

        private string _copyright;
        public string Copyright
        {
            get
            {
                return _copyright ?? string.Empty;
            }
            set
            {
                _copyright = value;
                OnPropertyChanged("Copyright");
            }
        }

        private string _company;
        public string Company
        {
            get
            {
                return _company ?? string.Empty;
            }
            set
            {
                _company = value;
                OnPropertyChanged("Company");
            }
        }

        private RelayCommand _okCommand;
        public RelayCommand OkCommand
        {
            get { return _okCommand ?? (_okCommand = new RelayCommand(e => CloseModal())); }
        }

        private RelayCommand _openCommand;
        public RelayCommand OpenCommand
        {
            get { return _openCommand ?? (_openCommand = new RelayCommand(e => OpenModal())); }
        }

        public AboutViewModel()
        {
            App.Mediator.Subscribe("MainWindow.About", this, OpenCommand);

            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var assembly = Assembly.GetExecutingAssembly();
            var customAttributes = assembly.GetCustomAttributes(false);
            foreach (var attribute in customAttributes)
            {
                if (attribute is AssemblyCopyrightAttribute)
                {
                    Copyright = ((AssemblyCopyrightAttribute)attribute).Copyright;
                }
                else if (attribute is AssemblyCompanyAttribute)
                {
                    Company = ((AssemblyCompanyAttribute)attribute).Company;
                }
            }

        }

        ~AboutViewModel()
        {
            App.Mediator.Unsubscribe(App._EVENT_MAINWINDOW_ABOUT, this, OpenCommand);
        }

        private void OpenModal()
        {
            IsVisible = true;
        }

        private void CloseModal()
        {
            IsVisible = false;
        }
    }
}
