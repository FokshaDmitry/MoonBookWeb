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

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ClientConnect.getInstance().onError += sayError;
            ClientConnect.getInstance().onAccess += sayAccess;
            ClientOperations.onRun += (msg) =>
            {
                Dispatcher.Invoke(() => lstAccess.Items.Add(msg));
            };
        }
        void sayAccess(string msg)
        {
            Dispatcher.Invoke(() => lstAccess.Items.Add(msg));

        }

        void sayError(string msg)
        {
            Dispatcher.Invoke(() => lstError.Items.Add(msg));
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClientConnect.getInstance().Connect();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ClientConnect.getInstance().Disconnect();
        }
    }
}
