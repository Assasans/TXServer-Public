using System;
using System.Collections.Generic;
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

        public override void OnSend()
        {
            // Add Entity to list.
            Player.Instance.EntityList.Add(Entity);

            EntityId = Entity.EntityId;
            TemplateAccessor = Entity.TemplateAccessor;

            foreach (Component component in Entity.Components)
            {
                Components.Add(component);
            }
        }

        public override void OnReceive()
        {
            throw new NotSupportedException();
        }

        [ProtocolIgnore] public Entity Entity { get; set; }

        [ProtocolFixed] public Int64 EntityId { get; set; }
        [ProtocolFixed][OptionalMapped] public TemplateAccessor TemplateAccessor { get; set; }
        [ProtocolFixed] public List<Component> Components { get; } = new List<Component>();
    }
}
