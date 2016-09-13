using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace ClientServerTest {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        Server server;

        public MainWindow () {
            InitializeComponent();
            stopButton.IsEnabled = false;
        }

        private void startButton_Click (object sender, RoutedEventArgs e) {
            int port;
            if (int.TryParse(portTextBox.Text, out port)) {
                server = new Server(IPAddress.Any, port);
                Server.OnServerNeedToLog += logServerResponse;
                server.ServerStart();
                startButton.IsEnabled = false;
                stopButton.IsEnabled = true;
            } else {
                Console.WriteLine("Wrong port: " + portTextBox.Text);
            }
        }

        private void stopButton_Click (object sender, RoutedEventArgs e) {
            if (server != null) {
                server.ServerStop();
                server = null;
            }
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
        }

        private void logServerResponse (string text) {
            Dispatcher.BeginInvoke(new ThreadStart(
                delegate {
                    logTextBox.AppendText(DateTime.Now.ToShortTimeString() + ": " + text + "\n");
                    logTextBox.ScrollToEnd();
                } 
            ));
        }

        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            if (server != null) {
                server.ServerStop();
                server.Dispose();
            }
        }

        private void logClearButton_Click (object sender, RoutedEventArgs e) {
            logTextBox.Text = string.Empty;
        }
    }
}
