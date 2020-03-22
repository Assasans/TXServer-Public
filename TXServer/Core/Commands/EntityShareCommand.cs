using System.IO;
using System;
using TXServer.Core.ECSSystem;
using System.Collections.Generic;
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

        public Entity Entity { get; set; }

        [Protocol] public Int64 EntityId { get; set; }
        [Protocol][OptionalMapped] public TemplateAccessor TemplateAccessor { get; set; }
        [Protocol] public List<Component> Components { get; } = new List<Component>();
    }
}
