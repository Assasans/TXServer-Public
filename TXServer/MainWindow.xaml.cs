﻿using System;
using System.Net;
using System.Net.NetworkInformation;
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

            UpdateStateText();
        }

        // Остановка сервера.
        public static void OnServerStop(string error = null)
        {
            if (error != null)
            {
                MessageBox.Show(error, "Ошибка");
            }
        }

        // Кнопка запуска сервера.
        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            ChangeServerState();
        }

        public void ChangeServerState()
        {
            Cursor = Cursors.Wait;
            if (!ServerLauncher.IsStarted)
            {
                SettingsGroupBox.IsEnabled = false;

                ServerLauncher.InitServer(IPAddressComboBox.SelectedItem as IPAddress, short.Parse(PortTextBox.Text), int.Parse(MaxPlayersTextBox.Text));

                StartButton.Content = "Остановить";
            }
            else
            {
                SettingsGroupBox.IsEnabled = true;

                ServerLauncher.StopServer();

                StartButton.Content = "Запуск";
            }

            Activate();
            UpdateStateText();
            Cursor = Cursors.Arrow;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ServerLauncher.IsStarted)
            {
                MessageBox.Show("Сначала остановите сервер.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Cancel = true;
            }
        }

        public void UpdateStateText()
        {
            if (!ServerLauncher.IsStarted)
            {
                ServerStateText.Text = "Сервер остановлен.";
                return;
            }

            ServerStateText.Text = "Игроков онлайн: " + ServerLauncher.PlayerCount;
        }
    }
}
