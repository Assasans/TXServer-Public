using System;
using System.Collections.Generic;
using System.Threading;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(2)]
    public class EntityShareCommand : ICommand
    {
        public EntityShareCommand(Entity Entity)
        {
            this.Entity = Entity ?? throw new ArgumentNullException(nameof(Entity));

            EntityId = Entity.EntityId;
            TemplateAccessor = Entity.TemplateAccessor;

            Components = new Component[Entity.Components.Count];
            Entity.Components.CopyTo((Component[])Components);
        }

        public void OnSend(Player player)
        {
            player.EntityList.Add(Entity);
            Entity.PlayerReferences.Add(player);
        }

        public void OnReceive(Player player)
        {
            throw new NotSupportedException();
        }

        private Entity Entity { get; }

        [ProtocolFixed] public Int64 EntityId { get; private set; }
        [ProtocolFixed][OptionalMapped] public TemplateAccessor TemplateAccessor { get; private set; }
        [ProtocolFixed] public IReadOnlyCollection<Component> Components { get; private set; }
    }
}
