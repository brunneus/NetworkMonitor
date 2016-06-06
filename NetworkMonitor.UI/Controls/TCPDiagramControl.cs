using NetworkMonitor.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkMonitor.UI.Controls
{
    public class TCPDiagramControl : Control
    {
        static TCPDiagramControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TCPDiagramControl), new FrameworkPropertyMetadata(typeof(TCPDiagramControl)));
        }

        public TCPPackage TCPPackage
        {
            get { return (TCPPackage)GetValue(TCPPackageProperty); }
            set { SetValue(TCPPackageProperty, value); }
        }

        public static readonly DependencyProperty TCPPackageProperty =
            DependencyProperty.Register("TCPPackage", typeof(TCPPackage), typeof(TCPDiagramControl));
    }
}
