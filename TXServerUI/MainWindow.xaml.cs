using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TXServer.Core;

namespace TXServerUI
{
    public partial class MainWindow : Window
    {
        private static volatile bool errorState;

        public MainWindow()
        {
            InitializeComponent();

            foreach (NetworkInterface @interface in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation ipInfo in @interface.GetIPProperties().UnicastAddresses)
                {
                    if (ipInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        IPAddressComboBox.Items.Add(ipInfo.Address);
                }
            }
            IPAddressComboBox.Items.Add(IPAddress.Any);
            IPAddressComboBox.SelectedItem = IPAddressComboBox.Items[0];

#if !DEBUG
            DebugOptionsGroupBox.Visibility = Visibility.Collapsed;
#endif

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
            ServerSettings settings = new()
            {
                IPAddress = (IPAddress)IPAddressComboBox.SelectedItem,
                Port = short.Parse(PortTextBox.Text),
                MaxPlayers = int.Parse(MaxPlayersTextBox.Text),

                DisableHeightMaps = DisableHeightMapsCheckBox.IsChecked.GetValueOrDefault(),
                TestServer = TestServerCheckBox.IsChecked.GetValueOrDefault(),

                DisablePingMessages = DisablePingMessagesCheckBox.IsChecked.GetValueOrDefault(),
                EnableTracing = EnableTracingCheckBox.IsChecked.GetValueOrDefault(),
                EnableCommandStackTrace = EnableCommandStackTraceCheckBox.IsChecked.GetValueOrDefault(),
            };

            ServerLauncher.InitServer(settings);

            SettingsStackPanel.IsEnabled = false;
            StartButton.Content = "Stop";
            Activate();
            _ = UpdateStateText();
        }

        private void StopServer()
        {
            ServerLauncher.StopServer();

            SettingsStackPanel.IsEnabled = true;
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

        private void EnableTracing_Click(object sender, RoutedEventArgs e)
        {
            TracingOptions.IsEnabled = EnableTracingCheckBox.IsChecked.GetValueOrDefault();
        }

        private async Task UpdateStateText()
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
