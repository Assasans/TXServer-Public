using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.Library;

namespace TXServer.Core
{
    /// <summary>
    /// Player connection.
    /// </summary>
    public sealed class Player : IDisposable
    {
        public Server Server { get; }
        public PlayerConnection Connection { get; }
        public PlayerData Data { get; set; }
        
        public Player(Server server, Socket socket)
        {
            Server = server;
            Connection = new PlayerConnection(this, socket);
        }

        public void Dispose()
        {
            if (Connection.IsActive()) return;
            
            Connection.Dispose();
        }

        public bool IsActive()
        {
            return Connection.IsActive();
        }

        /// <summary>
        /// Find Entity by id.
        /// </summary>
        public Entity FindById(Int64 id)
        {
            try
            {
                EntityList.TryGetValue(Entity.EqualValue(id), out Entity found);
                return found;
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Entity with id " + id + "not found.");
            }
        }

        public static Int64 GenerateId() => ((long)PlayerConnection.Random.Next() << 32) + PlayerConnection.Random.Next();

        public ConcurrentHashSet<Entity> EntityList { get; } = new ConcurrentHashSet<Entity>();

        /// <summary>
        /// Use for cross-Entity reference handling.
        /// </summary>
        public ConcurrentDictionary<string, Entity> ReferencedEntities { get; } = new ConcurrentDictionary<string, Entity>();
        
        //todo add those two in PlayerData
        public ConcurrentDictionary<string, ItemList> UserItems { get; } = new ConcurrentDictionary<string, ItemList>();
        public PresetEquipmentComponent CurrentPreset { get; set; }

        public Entity ClientSession { get; set; }

        public Entity User { get; set; }

        public string GetUniqueId()
        {
            return Data?.UniqueId;
        }
    }
}
