using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(5)]
    public class ComponentRemoveCommand : IComponentCommand
    {
        [ProtocolFixed] public Entity Target { get; set; }
        [ProtocolFixed] public Type ComponentType { get; set; }

        public ComponentRemoveCommand(Entity target, Type componentType)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            ComponentType = componentType ?? throw new ArgumentNullException(nameof(componentType));
        }

        public void OnReceive(Player player) => Target.RemoveComponent(ComponentType, player);

        public override string ToString() => $"ComponentRemoveCommand [Entity: {Target}, Component: {ComponentType.Name}]";
    }
}