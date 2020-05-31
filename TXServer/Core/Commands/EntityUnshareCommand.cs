using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(3)]
    public class EntityUnshareCommand : Command
    {
        public EntityUnshareCommand(Entity Entity)
        {
            this.Entity = Entity ?? throw new ArgumentNullException(nameof(Entity));
        }

        public override void OnSend(Player player)
        {
            player.EntityList.Remove(Entity);
        }

        public override void OnReceive(Player player)
        {
            throw new NotSupportedException();
        }

        [ProtocolFixed] public Entity Entity { get; set; }
    }
}
