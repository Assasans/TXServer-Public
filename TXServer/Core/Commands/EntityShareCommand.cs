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

        public override bool OnSend(Player player)
        {
            // Add Entity to list.
            player.EntityList.Add(Entity);

            lock (Entity.PlayerReferences)
            {
                if (Entity.PlayerReferences.ContainsKey(player))
                {
                    Entity.PlayerReferences[player]++;
                    return false;
                }
                else
                {
                    Entity.PlayerReferences.Add(player, 1);
                }
            }

            EntityId = Entity.EntityId;
            TemplateAccessor = Entity.TemplateAccessor;

            // Console.WriteLine("Sending " + EntityId + " (" + TemplateAccessor?.Template.GetType().Name + ")");
            
            foreach (Component component in Entity.Components)
            {
                // Console.WriteLine(component.GetType().Name);
                Components.Add(component);
            }

            return true;
        }

        public override void OnReceive(Player player)
        {
            throw new NotSupportedException();
        }

        [ProtocolIgnore] public Entity Entity { get; set; }

        [ProtocolFixed] public Int64 EntityId { get; set; }
        [ProtocolFixed][OptionalMapped] public TemplateAccessor TemplateAccessor { get; set; }
        [ProtocolFixed] public List<Component> Components { get; } = new List<Component>();
    }
}
