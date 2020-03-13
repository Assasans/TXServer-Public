using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using TXServer.Core;

namespace TXServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

#if DEBUG == false
            DebugGroupBox.Visibility = Visibility.Hidden;
#endif

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
        }

        // Остановка сервера.
        public static void OnServerStop(string error = null)
        {
            (Application.Current.MainWindow as MainWindow).ServerStopped(error);
        }

        // Обработка остановки сервера.
        public void ServerStopped(string error)
        {
            if (error != null)
            {
                MessageBox.Show(error, "Ошибка");
            }
        }

        // Кнопка запуска сервера.
        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            if (!ServerLauncher.IsStarted) {
                SettingsGroupBox.IsEnabled = false;
                StateGroupBox.IsEnabled = true;

                ServerLauncher.InitServer(IPAddressComboBox.SelectedItem as IPAddress, short.Parse(PortTextBox.Text), int.Parse(MaxPlayersTextBox.Text));

                StartButton.Content = "Остановить";
            }
            else
            {
                SettingsGroupBox.IsEnabled = true;
                StateGroupBox.IsEnabled = false;
                ServerStateText.Text = "";

                ServerLauncher.StopServer();

                StartButton.Content = "Запуск";
            }

            Activate();
            Cursor = Cursors.Arrow;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Core.ServerLauncher.IsStarted)
            {
                MessageBox.Show("Сначала остановите сервер.", "Ошибка");
                e.Cancel = true;
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ShowStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ServerLauncher.IsStarted) return;

            ServerStateText.Text = "Игроков онлайн: " + ServerLauncher.PlayerCount;
        }
    }
}
