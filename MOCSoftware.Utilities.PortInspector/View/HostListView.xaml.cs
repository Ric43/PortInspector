using System.Windows;
using System.Windows.Controls;

namespace MOCSoftware.Utilities.PortInspector.View
{
    /// <summary>
    /// Interaction logic for HostListView.xaml
    /// </summary>
    public partial class HostListView : UserControl
    {
        public HostListView()
        {
            InitializeComponent();
        }

        private void btnAddHost_Click(object sender, RoutedEventArgs e)
        {
            var bindingExpression = txtHostName.GetBindingExpression(TextBox.TextProperty);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateSource();
                txtHostName.Text = string.Empty;
            }
        }
    }
}
