using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(3)]
    public class EntityUnshareCommand : ICommand
    {
        [ProtocolFixed] public Entity Entity { get; }

        public EntityUnshareCommand(Entity entity)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
        }

        public void OnReceive(Player player)
        {
            throw new NotSupportedException("Client by itself cannot share entities.");
        }

        public override string ToString() => $"EntityUnshareCommand [Entity: {Entity}]";
    }
}
