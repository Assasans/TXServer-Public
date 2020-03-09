using System.Net.Sockets;
using System.Threading;

namespace TXServer.Core
{
    // Описание игрока в пуле.
    public class PlayerConnection
    {

        public PlayerConnection(int id)
        {
            UpWorker.Name = "ServerSide #" + id;
            UpWorker.Start(data);

            DownWorker.Name = "ClientSide #" + id;
            DownWorker.Start(data);
        }

        public void Destroy()
        {
            UpWorker.Abort();
            DownWorker.Abort();

            if (data.Socket != null)
                data.Socket.Close();
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
            data.Socket = socket;
            PlayerData.Instance = data;

            UpdateState();

            Interlocked.Increment(ref Core.PlayerCount);
        }

        public PlayerData data = new PlayerData();

        private Thread UpWorker = new Thread(PlayerHandlers.ServerSideEvents);
        private Thread DownWorker = new Thread(PlayerHandlers.ClientSideEvents);
    }
}
