using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MOCSoftware.Utilities.PortInspector.Model;

namespace MOCSoftware.Utilities.PortInspector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand AddPortCommand = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
