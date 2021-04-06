using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    public abstract class ComponentAddOrChangeCommand : IComponentCommand
    {
        [ProtocolFixed] public Entity Target { get; set; }
        [ProtocolFixed] public Component Component { get; set; }

        public ComponentAddOrChangeCommand(Entity Target, Component Component)
        {
            this.Target = Target;
            this.Component = Component;
        }

        public virtual void OnReceive(Player player) => AddOrChangeComponent(player);

        protected abstract void AddOrChangeComponent(Player player);

        public override string ToString() => $"{GetType().Name} [Entity: {Target}, Component: {Component}]";
    }
}