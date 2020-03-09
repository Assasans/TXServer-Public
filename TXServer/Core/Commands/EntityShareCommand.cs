using System.IO;
using System;
using TXServer.Core.ECSSystem;
using System.Collections.Generic;
using static TXServer.Core.ECSSystem.Components;

namespace TXServer.Core.Commands
{
    public class EntityShareCommand : Command
    {
        public EntityShareCommand() { }

        public EntityShareCommand(Entity Entity)
        {
            this.Entity = Entity;
        }

        public override void BeforeWrap()
        {
            UInt64 newEntityId = Player.Instance.Value.GenerateId();

            // Добавить Entity в общий список.
            Player.Instance.Value.EntityList[newEntityId] = Entity;
            Player.Instance.Value.EntityIds[Entity] = newEntityId;

            EntityId = newEntityId;
            TemplateAccessor = Entity.TemplateAccessor;

            foreach (Component component in Entity.Components.Values)
            {
                Components.Add(component);
            }
        }

        public Entity Entity { get; set; }

        [Protocol] public UInt64 EntityId { get; set; }
        [Protocol][OptionalMapped] public TemplateAccessor TemplateAccessor { get; set; }
        [Protocol] public List<Component> Components { get; set; } = new List<Component>();
    }
}
