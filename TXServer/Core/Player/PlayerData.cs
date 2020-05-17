using System;
using System.Collections.Concurrent;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.Library;

namespace TXServer.Core
{
    public partial class Player
    {
        [ThreadStatic] private static Player _Instance;
        public static Player Instance => _Instance;

        [ThreadStatic] private static Random Random;
        public static Int64 GenerateId() => ((long)Random.Next() << 32) + Random.Next();

        public ConcurrentHashSet<Entity> EntityList { get; } = new ConcurrentHashSet<Entity>();
        /// <summary>
        /// Use for cross-Entity reference handling.
        /// </summary>
        public ConcurrentDictionary<string, Entity> ReferencedEntities { get; } = new ConcurrentDictionary<string, Entity>();
        
        public Entity ClientSession { get; set; }
        public Entity User { get; set; }

        public string Uid { get; set; }

        public ConcurrentDictionary<string, ItemList> UserItems { get; } = new ConcurrentDictionary<string, ItemList>();
        public PresetEquipmentComponent CurrentPreset { get; set; }
    }
}
