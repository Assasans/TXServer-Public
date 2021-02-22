using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(3)]
    public class EntityUnshareCommand : ICommand
    {
        public EntityUnshareCommand(Entity Entity, bool isManual = false)
        {
            this.Entity = Entity ?? throw new ArgumentNullException(nameof(Entity));
            this.isManual = isManual;
        }

        public void OnSend(Player player)
        {
            if (isManual) return;
            player.RemoveEntity(Entity);
        }

        public void OnReceive(Player player)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return $"EntityUnshareCommand [Entity: {Entity}]";
        }

        [ProtocolFixed] public Entity Entity { get; }
        private readonly bool isManual;
    }
}
