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

            EntityId = Entity.EntityId;
            TemplateAccessor = Entity.TemplateAccessor;

            Components = new Component[Entity.Components.Count];
            Entity.Components.CopyTo((Component[])Components);
        }

        public override bool OnSend(Player player)
        {
            player.EntityList.Add(Entity);
            Entity.PlayerReferences.Add(player);
            return true;
        }

        public override void OnReceive(Player player)
        {
            throw new NotSupportedException();
        }

        private Entity Entity { get; }

        [ProtocolFixed] public Int64 EntityId { get; private set; }
        [ProtocolFixed][OptionalMapped] public TemplateAccessor TemplateAccessor { get; private set; }
        [ProtocolFixed] public IReadOnlyCollection<Component> Components { get; private set; }
    }
}
