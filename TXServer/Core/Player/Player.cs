using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace TXServer.Core
{
    /// <summary>
    /// Player connection.
    /// </summary>
    [DebuggerDisplay("Active = {Active}, LoggedIn = {User != null}")]
    public sealed partial class Player : IDisposable
    {
        public Player(Socket Socket)
        {
            this.Socket = Socket ?? throw new ArgumentNullException(nameof(Socket));

            Interlocked.Increment(ref ServerLauncher.PlayerCount);
            Application.Current.Dispatcher.Invoke(new Action(() => { (Application.Current.MainWindow as MainWindow).UpdateStateText(); }));

            // Start player threads.
            UpWorker = new Thread(() => ServerSideEvents());
            UpWorker.Name = "ServerSide #" + Socket.Handle;
            UpWorker.Start();

            DownWorker = new Thread(() => ClientSideEvents());
            DownWorker.Name = "ClientSide #" + Socket.Handle;
            DownWorker.Start();
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _Active, 0) == 0) return;

            Socket.Disconnect(false);

            Interlocked.Decrement(ref ServerLauncher.PlayerCount);
            Application.Current.Dispatcher.Invoke(new Action(() => { (Application.Current.MainWindow as MainWindow).UpdateStateText(); }));
        }

        public Socket Socket { get; private set; }

        public bool Active => Convert.ToBoolean(_Active);
        private int _Active = 1;

        private readonly Thread UpWorker;
        private readonly Thread DownWorker;
    }
}
