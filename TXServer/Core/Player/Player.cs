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

            Console.WriteLine($"{Socket.RemoteEndPoint}: initializing player.");

            Interlocked.Increment(ref ServerLauncher.PlayerCount);
            Application.Current.Dispatcher.Invoke(new Action(() => { (Application.Current.MainWindow as MainWindow).UpdateStateText(); }));

            // Start player threads.
            new Thread(() => ServerSideEvents())
            {
                Name = $"{Socket.RemoteEndPoint} (Server side)"
            }.Start();

            new Thread(() => ClientSideEvents())
            {
                Name = $"{Socket.RemoteEndPoint} (Client side)"
            }.Start();
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _Active, 0) == 0)
            {
                Console.WriteLine($"{Socket.RemoteEndPoint} is disconnected.");
                return;
            }
            Console.WriteLine($"{Socket.RemoteEndPoint} is disconnecting.");

            Socket.Disconnect(false);

            // Any data should be saved before this comment.
            LobbyCommandQueue = null;
            BattleCommandQueue = null;
            EntityList.Clear();
            UserItems.Clear();
            CurrentPreset = null;

            Interlocked.Decrement(ref ServerLauncher.PlayerCount);
            Application.Current.Dispatcher.Invoke(new Action(() => { (Application.Current.MainWindow as MainWindow).UpdateStateText(); }));
        }

        public Socket Socket { get; private set; }

        public bool Active => Convert.ToBoolean(_Active);
        private int _Active = 1;
    }
}
