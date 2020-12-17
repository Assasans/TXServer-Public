using System;
using System.Collections.Generic;
using System.Threading;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(2)]
    public class EntityShareCommand : Command
    {
        public EntityShareCommand(Entity Entity)
        {
            this.Entity = Entity ?? throw new ArgumentNullException(nameof(Entity));
        }

        public override bool OnSend(Player player)
        {
            // Add Entity to list.
            player.EntityList.Add(Entity);

            lock (Entity.PlayerReferences)
            {
                if (Entity.PlayerReferences.ContainsKey(player))
                {
                    // Player already has this entity
                    Entity.PlayerReferences[player]++;
                    return false;
                }
                else
                {
                    Entity.PlayerReferences.Add(player, 1);
                }
            }

            if (Interlocked.Exchange(ref IsFilled, 1) == 0)
            {
                EntityId = Entity.EntityId;
                TemplateAccessor = Entity.TemplateAccessor;

                Components = new Component[Entity.Components.Count];
                Entity.Components.CopyTo((Component[])Components);
            }

            return true;
        }

        public override void OnReceive(Player player)
        {
            throw new NotSupportedException();
        }

        private Entity Entity { get; }
        private int IsFilled;

        [ProtocolFixed] public Int64 EntityId { get; private set; }
        [ProtocolFixed][OptionalMapped] public TemplateAccessor TemplateAccessor { get; private set; }
        [ProtocolFixed] public ICollection<Component> Components { get; private set; }
    }
}
