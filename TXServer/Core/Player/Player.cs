using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace TXServer.Core
{
    // Соединение с игроком.
    public partial class Player
    {
        public Player(Socket Socket)
        {
            this.Socket = Socket ?? throw new ArgumentNullException(nameof(Socket));
            Instance = this;

            Random = new System.Random(Socket.Handle.ToInt32());

            Interlocked.Increment(ref ServerLauncher.PlayerCount);
            Application.Current.Dispatcher.Invoke(new Action(() => { (Application.Current.MainWindow as MainWindow).UpdateState(); }));

            UpWorker = new Thread(ServerSideEvents);
            UpWorker.Name = "ServerSide #" + Socket.Handle;
            UpWorker.Start(this);

            DownWorker = new Thread(ClientSideEvents);
            DownWorker.Name = "ClientSide #" + Socket.Handle;
            DownWorker.Start(this);
        }

        public void Destroy()
        {
            lock (this)
            {
                if (Socket != null)
                {
                    Socket.Close();
                    Socket = null;
                }
            }

            Interlocked.Decrement(ref ServerLauncher.PlayerCount);
            Application.Current.Dispatcher.Invoke(new Action(() => { (Application.Current.MainWindow as MainWindow).UpdateState(); }));
        }

        public Socket Socket { get; private set; }

        public bool IsBusy { get; private set; } = true;

        private readonly Thread UpWorker;
        private readonly Thread DownWorker;
    }
}
