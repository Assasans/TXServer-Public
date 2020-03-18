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
            Int64 newEntityId = Player.GenerateId();

            // Добавить Entity в общий список.
            Player.Instance.EntityList[newEntityId] = Entity;

            EntityId = newEntityId;
            TemplateAccessor = Entity.TemplateAccessor;

            foreach (Component component in Entity.Components.Values)
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
