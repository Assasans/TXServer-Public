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
        public EntityShareCommand(Entity Entity, bool isManual = false)
        {
            this.Entity = Entity ?? throw new ArgumentNullException(nameof(Entity));

            EntityId = Entity.EntityId;
            TemplateAccessor = Entity.TemplateAccessor;

            Components = new Component[Entity.Components.Count];
            Entity.Components.CopyTo((Component[])Components);

            this.isManual = isManual;
        }

        public void OnSend(Player player)
        {
            if (isManual) return;
            player.AddEntity(Entity);
        }

        public void OnReceive(Player player)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return $"EntityShareCommand [Entity: {Entity}]";
        }

        private Entity Entity { get; }
        private readonly bool isManual;

        [ProtocolFixed] public Int64 EntityId { get; private set; }
        [ProtocolFixed][OptionalMapped] public TemplateAccessor TemplateAccessor { get; private set; }
        [ProtocolFixed] public IReadOnlyCollection<Component> Components { get; private set; }
    }
}
