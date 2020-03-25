using System;
using System.Collections.Generic;
using TXServer.Core.ECSSystem;
using TXServer.Core.ECSSystem.Components;

namespace TXServer.Core.Commands
{
    public class EntityShareCommand : Command
    {
        public EntityShareCommand(Entity Entity)
        {
            this.Entity = Entity;
        }

        public override void OnSend()
        {
            // Добавить Entity в общий список.
            Player.Instance.EntityList.Add(Entity);

            EntityId = Entity.EntityId;
            TemplateAccessor = Entity.TemplateAccessor;

            foreach (Component component in Entity.Components)
            {
                Components.Add(component);
            }
        }

        [ProtocolIgnore] public Entity Entity { get; set; }

        [ProtocolFixed] public Int64 EntityId { get; set; }
        [ProtocolFixed][OptionalMapped] public TemplateAccessor TemplateAccessor { get; set; }
        [ProtocolFixed] public List<Component> Components { get; } = new List<Component>();
    }
}
