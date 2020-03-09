using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using TXServer.Bits;
using TXServer.Core.ECSSystem;

namespace TXServer.Core
{
    public class PlayerData
    {
        public void Free()
        {
            Socket.Close();

            Interlocked.Decrement(ref Core.PlayerCount);
        }

        private static ThreadLocal<PlayerData> _Instances = new ThreadLocal<PlayerData>();
        public static PlayerData Instance
        {
            get
            {
                return _Instances.Value;
            }
            set
            {
                _Instances.Value = value;
            }
        }

        public Socket Socket { get; set; }

        // Генератор случайных значений.
        private readonly Random Random = new Random(SafeRandom.Next());
        public ulong GenerateId() => ((ulong)Random.Next() << 32) + (ulong)Random.Next();

        // Entity list.
        public readonly ConcurrentDictionary<ulong, Entity> EntityList = new ConcurrentDictionary<ulong, Entity>();
        public readonly ConcurrentDictionary<Entity, ulong> EntityIds = new ConcurrentDictionary<Entity, ulong>();
    }
}
