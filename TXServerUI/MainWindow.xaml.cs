using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using TXServer.Core;

namespace TXServerUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static volatile bool errorState;
        
        public MainWindow()
        {
            InitializeComponent();

            // Заполнение списка IP-адресов.
            foreach (NetworkInterface @interface in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation ipInfo in @interface.GetIPProperties().UnicastAddresses)
                {
                    if (ipInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        IPAddressComboBox.Items.Add(ipInfo.Address);
                    }
                }
            }

            IPAddressComboBox.Items.Add(IPAddress.Any);

            UpdateStateText();
        }

        public void HandleCriticalError()
        {
            if (errorState) return;
            errorState = true;

            MessageBox.Show("An error occured while starting server. Try running application as administrator.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            StopServer();
        }

        // Кнопка запуска сервера.
        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            errorState = false;
            ChangeServerState();
        }

        public void ChangeServerState()
        {
            Cursor = Cursors.Wait;
            if (!ServerLauncher.IsStarted())
                StartServer();
            else
                StopServer();
            
            Cursor = Cursors.Arrow;
        }

        private void StartServer()
        {
            SettingsGroupBox.IsEnabled = false;

            ServerLauncher.InitServer((IPAddress)IPAddressComboBox.SelectedItem, short.Parse(PortTextBox.Text), int.Parse(MaxPlayersTextBox.Text));
            TXServer.Core.Commands.CommandManager.EnableTracing = EnableTracingCheckBox.IsChecked.GetValueOrDefault();

            StartButton.Content = "Stop";

            Activate();
            new Thread(UpdateStateText).Start();
        }

        private void StopServer()
        {
            SettingsGroupBox.IsEnabled = true;

            ServerLauncher.StopServer();

            StartButton.Content = "Start";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ServerLauncher.IsStarted())
            {
                MessageBox.Show("Stop server first.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Cancel = true;
            }
        }
        
        public void UpdateStateText()
        {
            while (ServerLauncher.IsStarted())
            {
                if (Server.Instance.Connection.IsError)
                {
                    ServerLauncher.StopServer(false);
                    Application.Current.Dispatcher.Invoke(HandleCriticalError);
                    break;
                }

                Application.Current.Dispatcher.Invoke(() =>
                    ServerStateText.Text = $"Connected players: {ServerLauncher.GetPlayerCount()}\n" +
                        $"Active battles: {ServerConnection.BattlePool.Count}\n" +
                        $"Last tick duration: {ServerConnection.LastTickDuration}");
                Thread.Sleep(1000);
            }
            
            Application.Current.Dispatcher.Invoke(() => ServerStateText.Text = "Server is stopped.");
            return;
        }
    }
}
