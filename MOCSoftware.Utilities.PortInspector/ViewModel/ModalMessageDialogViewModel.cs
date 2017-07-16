using MOCSoftware.Utilities.PortInspector.Communication;
using MOCSoftware.Utilities.PortInspector.Utility;

namespace MOCSoftware.Utilities.PortInspector.ViewModel
{
    internal class ModalMessageDialogViewModel : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        private RelayCommand _okCommand;
        public RelayCommand OkCommand
        {
            get { return _okCommand ?? (_okCommand = new RelayCommand(e => CloseModal())); }
        }

        private RelayCommand _openCommand;
        internal RelayCommand OpenCommand
        {
            get { return _openCommand ?? (_openCommand = new RelayCommand(e => OpenModal((ErrorMessage)e))); }
        }

        internal ModalMessageDialogViewModel()
        {
            App.Mediator.Subscribe(App._EVENT_ERROR, this, OpenCommand);
        }

        ~ModalMessageDialogViewModel()
        {
            App.Mediator.Unsubscribe(App._EVENT_ERROR, this, OpenCommand);
        }

        private void OpenModal(ErrorMessage message)
        {
            Title = message.ErrorTitle;
            Message = message.ErrorText;
            IsVisible = true;
        }

        private void CloseModal()
        {
            Title = string.Empty;
            Message = string.Empty;
            IsVisible = false;
        }
    }
}
