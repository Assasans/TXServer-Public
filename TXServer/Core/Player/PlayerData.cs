using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using TXServer.Bits;
using TXServer.Core.ECSSystem;

namespace TXServer.Core
{
    public partial class Player
    {
        private static ThreadLocal<Player> _Instances = new ThreadLocal<Player>();
        public static Player Instance
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

        // Генератор случайных значений.
        private readonly Random Random;
        public ulong GenerateId() => ((ulong)Random.Next() << 32) + (ulong)Random.Next();

        // Entity list.
        public ConcurrentDictionary<ulong, Entity> EntityList { get; } = new ConcurrentDictionary<ulong, Entity>();
        public ConcurrentDictionary<Entity, ulong> EntityIds { get; } = new ConcurrentDictionary<Entity, ulong>();
    }
}
