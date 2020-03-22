using System;
using TXServer.Core.ECSSystem;
using TXServer.Library;

namespace TXServer.Core
{
    public partial class Player
    {
        [ThreadStatic] private static Player _Instance;
        public static Player Instance => _Instance;

        // Генератор случайных значений.
        [ThreadStatic] private static Random Random;
        public static Int64 GenerateId() => ((long)Random.Next() << 32) + Random.Next();

        // Entity list.
        public ConcurrentHashSet<Entity> EntityList { get; } = new ConcurrentHashSet<Entity>();
    }
}
