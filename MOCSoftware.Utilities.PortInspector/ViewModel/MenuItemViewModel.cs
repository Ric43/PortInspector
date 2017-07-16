using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;

namespace MOCSoftware.Utilities.PortInspector.ViewModel
{
    [ContentProperty("MenuItems")]
    public class MenuItemViewModel : INotifyPropertyChanged, IEquatable<MenuItemViewModel>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand _command;
        private string _header;
        private string _parent;
        private string _contextMenuName;
        private int _menuItemPriority = 0;
        private string _tag = string.Empty;
        private bool _isEnabled = true;
        private bool _isSeparator = false;

        public ICommand Command
        {
            get { return _command; }
            set
            {
                _command = value;
                OnPropertyChanged("Command");
            }
        }
        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                OnPropertyChanged("Header");
            }
        }
        public string Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                OnPropertyChanged("Parent");
            }
        }
        public string ContextMenuName
        {
            get { return _contextMenuName; }
            set
            {
                _contextMenuName = value;
                OnPropertyChanged("ContextMenuName");
            }
        }
        public int Priority
        {
            get { return _menuItemPriority; }
            set
            {
                _menuItemPriority = value;
                OnPropertyChanged("Priority");
            }
        }
        public string Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                OnPropertyChanged("Tag");
            }
        }
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        public bool IsSeparator
        {
            get { return _isSeparator; }
            set
            {
                _isSeparator = value;
                OnPropertyChanged("IsSeparator");
            }
        }

        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

        public MenuItemViewModel()
        {
            MenuItems = new ObservableCollection<MenuItemViewModel>();
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool Equals(MenuItemViewModel other)
        {
            return Header.ToLower().Equals(other.Header.ToLower());
        }
    }
}
