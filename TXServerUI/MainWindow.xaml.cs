using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
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

            _ = UpdateStateText();
        }

        public void HandleCriticalError()
        {
            if (errorState) return;
            errorState = true;

            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("An error occured while starting server. Try running application as administrator.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                StopServer();
            });
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

            ServerSettings settings = new()
            {
                IPAddress = (IPAddress)IPAddressComboBox.SelectedItem,
                Port = short.Parse(PortTextBox.Text),
                MaxPlayers = int.Parse(MaxPlayersTextBox.Text),
                DisableHeightMaps = DisableHeightMapsCheckBox.IsChecked.GetValueOrDefault(),
                TraceModeEnabled = EnableTracingCheckBox.IsChecked.GetValueOrDefault()
            };

            ServerLauncher.InitServer(settings);

            StartButton.Content = "Stop";

            Activate();
            _ = UpdateStateText();
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

        public async Task UpdateStateText()
        {
            while (ServerLauncher.IsStarted())
            {
                ServerStateText.Text = $"Connected players: {ServerLauncher.GetPlayerCount()}\n" +
                    $"Active battles: {ServerConnection.BattlePool.Count}\n" +
                    $"Last tick duration: {ServerConnection.LastTickDuration}\n";
                if (Server.Instance.StoredPlayerData.Count > 0)
                    ServerStateText.Text += $"Stored player data: {Server.Instance.StoredPlayerData.Count}\n";
                await Task.Delay(1000);
            }

            ServerStateText.Text = "Server is stopped.";
        }
    }
}
