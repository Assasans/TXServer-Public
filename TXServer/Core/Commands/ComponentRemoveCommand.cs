using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(5)]
    public class ComponentRemoveCommand : IComponentCommand
    {
        public ComponentRemoveCommand(Entity target, Type componentType)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            ComponentType = componentType ?? throw new ArgumentNullException(nameof(componentType));
        }

        public ComponentRemoveCommand(Entity target, Type componentType, bool removeDone) : this(target, componentType)
        {
            this.removeDone = Convert.ToInt32(removeDone);
        }

        public void OnSend(Player player)
        {
            RemoveComponent(player);
        }

        public void OnReceive(Player player) => RemoveComponent(player);

        private void RemoveComponent(Player player)
        {
            if (Interlocked.Exchange(ref removeDone, 1) == 1) return;
            lock (Target.PlayerReferences)
                CommandManager.BroadcastCommands(Target.PlayerReferences.Where(x => x != player), this);

            Target.RemoveComponentLocally(ComponentType);
        }

        public override string ToString()
        {
            return $"ComponentRemoveCommand [Entity: {Target}, Component: {ComponentType.Name}]";
        }

        [ProtocolFixed] public Entity Target { get; set; }
        [ProtocolFixed] public Type ComponentType { get; set; }

        private int removeDone;
    }
}