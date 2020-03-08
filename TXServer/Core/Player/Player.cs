using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using TXServer.Bits;
using TXServer.Core.ECSSystem;

namespace TXServer.Core
{
    // Описание игрока в пуле.
    public partial class Player
    {
        // Состояние игрока.
        public enum PlayerState
        {
            Disconnected,
            Crashed,
            LoginScreen,
            LoggedIn
        }

        public Player(int id)
        {
            UpWorker.Name = "ServerSideEvents #" + id;
            UpWorker.Start(this);

            DownWorker.Name = "ClientSideEvents #" + id;
            DownWorker.Start(this);
        }

        public void Destroy()
        {
            UpWorker.Abort();
            DownWorker.Abort();

            try
            {
                Socket.Close();
            }
            catch { }
        }

        public void Free()
        {
            State = PlayerState.Disconnected;

            try
            {
                Socket.Close();
            }
            catch { }

            UpdateState();

            EntityIds.Clear();
            EntityList.Clear();

            Interlocked.Decrement(ref PlayerCount);
        }

        // Обновление состояния потоков.
        private void UpdateState()
        {
            UpWorker.Interrupt();
            DownWorker.Interrupt();
        }

        // Подготовка соединения с игроком.
        public void Prepare(Socket socket)
        {
            State = PlayerState.LoginScreen;

            Socket = socket;
            UpdateState();

            Interlocked.Increment(ref PlayerCount);
        }


        // Количество игроков на сервере.
        public static int PlayerCount;

        // Соединение с клиентом.
        public Socket Socket;

        private Thread UpWorker = new Thread(ClientHandlers.ServerSideEvents);
        private Thread DownWorker = new Thread(ClientHandlers.ClientSideEvents);

        public PlayerState State;

        // Генератор случайных значений.
        private readonly Random Random = new Random(SafeRandom.Next());
        public UInt64 GenerateId() => ((UInt64)Random.Next() << 32) + (UInt64)Random.Next();

        // Entity list.
        public readonly ConcurrentDictionary<UInt64, Entity> EntityList = new ConcurrentDictionary<UInt64, Entity>();
        public readonly ConcurrentDictionary<Entity, UInt64> EntityIds = new ConcurrentDictionary<Entity, UInt64>();
    }
}
