using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
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

        public override bool OnSend(Player player)
        {
            RemoveComponent(player);
            return true;
        }

        public override void OnReceive(Player player) => RemoveComponent(player);

        // TODO: keep track of players entity shared with and broadcast any changes

        private void RemoveComponent(Player player)
        {
            if (Interlocked.Exchange(ref removeDone, 1) == 1) return;
            CommandManager.BroadcastCommands(Target.PlayerReferences.Keys.Where(x => x != player), this);

            if (!Target.Components.Remove(FormatterServices.GetUninitializedObject(ComponentType) as Component))
            {
                throw new ArgumentException("Component not found", ComponentType.Name);
            }
        }

        [ProtocolFixed] public Entity Target { get; set; }
        [ProtocolFixed] public Type ComponentType { get; set; }

        private int removeDone;
    }
}