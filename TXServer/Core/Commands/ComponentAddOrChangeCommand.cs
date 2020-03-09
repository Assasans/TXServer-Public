using TXServer.Bits;
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

        public override void BeforeWrap()
        {
            AddOrChangeComponent();
        }

        public override void AfterUnwrap()
        {
            AddOrChangeComponent();
        }

        protected abstract void AddOrChangeComponent();

        [Protocol] public Entity Target { get; set; }
        [Protocol] public Component Component { get; set; }
    }
}