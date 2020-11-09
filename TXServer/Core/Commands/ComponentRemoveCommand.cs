using System;
using System.Runtime.Serialization;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(5)]
    public class ComponentRemoveCommand : Command
    {
        public ComponentRemoveCommand(Entity target, Type componentType)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            ComponentType = componentType ?? throw new ArgumentNullException(nameof(componentType));
        }

        public override void OnSend(Player player) => RemoveComponent();

        public override void OnReceive(Player player) => RemoveComponent();

        // TODO: keep track of players entity shared with and broadcast any changes

        private void RemoveComponent()
        {
            if (!Target.Components.Remove(FormatterServices.GetUninitializedObject(ComponentType) as Component))
            {
                throw new ArgumentException("Component not found", ComponentType.Name);
            }
        }

        [ProtocolFixed] public Entity Target { get; set; }
        [ProtocolFixed] public Type ComponentType { get; set; }
    }
}