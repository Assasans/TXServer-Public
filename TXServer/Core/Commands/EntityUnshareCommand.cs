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

        public override bool OnSend(Player player)
        {
            player.EntityList.Remove(Entity);

            lock (Entity.PlayerReferences)
            {
                if (Entity.PlayerReferences[player] == 1)
                {
                    Entity.PlayerReferences.Remove(player);
                }
                else
                {
                    Entity.PlayerReferences[player]--;
                    return false;
                }
            }

            return true;
        }

        public override void OnReceive(Player player)
        {
            throw new NotSupportedException();
        }

        [ProtocolFixed] public Entity Entity { get; }
    }
}
