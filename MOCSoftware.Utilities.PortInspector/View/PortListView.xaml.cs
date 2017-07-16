using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using MOCSoftware.Utilities.PortInspector.Model;

namespace MOCSoftware.Utilities.PortInspector.View
{
    /// <summary>
    /// Interaction logic for PortListView.xaml
    /// </summary>
    public partial class PortListView : UserControl
    {
        public PortListView()
        {
            InitializeComponent();
        }

        private void OnPortListRowMouseEnter(object sender, MouseEventArgs e)
        {
            var row = (DataGridRow)sender;
            var dataItem = (PortInfo)row.Item;
            var statusText = new StringBuilder();
            if (dataItem.Alias.Trim().Length > 0)
                statusText.AppendFormat("Alias: {0}\t", dataItem.Alias.Trim());
            if (dataItem.Description.Trim().Length > 0)
                statusText.AppendFormat("Description: {0}", dataItem.Description);
            App.Mediator.Publish(App._EVENT_STATUSUPDATE, new Communication.StatusUpdateMessage{StatusMessage = statusText.ToString()});
        }

        private void OnPortListRowMouseLeave(object sender, MouseEventArgs e)
        {
            App.Mediator.Publish(App._EVENT_STATUSUPDATE, new Communication.StatusUpdateMessage { StatusMessage = string.Empty });
        }
    }
}
