﻿using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Input;
using TXServer.Core;

namespace TXServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static volatile bool errorState;
        
        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru");

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

        public static void HandleCriticalError()
        {
            if (errorState) return;
            errorState = true;

            MessageBox.Show("An error occured while starting server. Try running application as administrator.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            (Application.Current.MainWindow as MainWindow).ChangeServerState();
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
            {
                SettingsGroupBox.IsEnabled = false;

                ServerLauncher.InitServer(IPAddressComboBox.SelectedItem as IPAddress, short.Parse(PortTextBox.Text), int.Parse(MaxPlayersTextBox.Text));

                StartButton.Content = "Stop";
            }
            else
            {
                SettingsGroupBox.IsEnabled = true;

                ServerLauncher.StopServer();

                StartButton.Content = "Start";
            }

            Activate();
            UpdateStateText();
            Cursor = Cursors.Arrow;
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
            if (!ServerLauncher.IsStarted())
            {
                ServerStateText.Text = "Server is stopped.";
                return;
            }

            ServerStateText.Text = $"Online players: {ServerLauncher.GetPlayerCount()}\n" +
                $"Last tick duration: {ServerConnection.LastTickDuration}";
        }
    }
}
