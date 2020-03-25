using TXServer.Core.ECSSystem;
using TXServer.Core.ECSSystem.Components;

namespace TXServer.Core.Commands
{
    public abstract class ComponentAddOrChangeCommand : Command
    {
        public ComponentAddOrChangeCommand() { }

        public ComponentAddOrChangeCommand(Entity Target, Component Component)
        {
            this.Target = Target;
            this.Component = Component;
        }

        public override void OnSend()
        {
            AddOrChangeComponent();
        }

        public override void OnReceive()
        {
            AddOrChangeComponent();
        }

        protected abstract void AddOrChangeComponent();

        [ProtocolFixed] public Entity Target { get; set; }
        [ProtocolFixed] public Component Component { get; set; }
    }
}