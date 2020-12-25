using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(3)]
    public class EntityUnshareCommand : ICommand
    {
        public EntityUnshareCommand(Entity Entity)
        {
            this.Entity = Entity ?? throw new ArgumentNullException(nameof(Entity));
        }

        public void OnSend(Player player)
        {
            player.EntityList.Remove(Entity);
            Entity.PlayerReferences.Remove(player);
        }

        public void OnReceive(Player player)
        {
            throw new NotSupportedException();
        }

        [ProtocolFixed] public Entity Entity { get; }
    }
}
