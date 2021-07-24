using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(2)]
    public class EntityShareCommand : ICommand
    {
        private Entity Entity { get; }

        [ProtocolFixed] public Int64 EntityId { get; private set; }
        [ProtocolFixed] [OptionalMapped] public TemplateAccessor TemplateAccessor { get; private set; }
        [ProtocolFixed] public IReadOnlyCollection<Component> Components { get; }

        public EntityShareCommand(Entity entity, Player player)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));

            EntityId = entity.EntityId;
            TemplateAccessor = entity.TemplateAccessor;

            Component[] shareAbleComponents =
                Entity.Components.Where(c => c.SelfOnlyPlayer is null || c.SelfOnlyPlayer == player).ToArray();

            Components = new Component[shareAbleComponents.Length];
            shareAbleComponents.CopyTo((Component[])Components, 0);
        }

        public void OnReceive(Player player)
        {
            throw new NotSupportedException();
        }

        public override string ToString() => $"EntityShareCommand [Entity: {Entity}]";
    }
}
