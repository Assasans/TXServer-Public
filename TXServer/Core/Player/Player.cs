using System;
using System.Net.Sockets;
using System.Threading;

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

            UpWorker = new Thread(ServerSideEvents);
            UpWorker.Name = "ServerSide #" + Socket.Handle;
            UpWorker.Start(this);

            DownWorker = new Thread(ClientSideEvents);
            DownWorker.Name = "ClientSide #" + Socket.Handle;
            DownWorker.Start(this);
        }

        public void Destroy()
        {
            lock (Instance)
            {
                Socket.Close();
                Socket = null;
            }

            Instance = null;

            Interlocked.Decrement(ref ServerLauncher.PlayerCount);
        }

        public Socket Socket { get; private set; }

        public bool IsBusy { get; private set; } = true;

        private readonly Thread UpWorker;
        private readonly Thread DownWorker;
    }
}
