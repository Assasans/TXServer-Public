using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    public abstract class ComponentAddOrChangeCommand : Command
    {
        public ComponentAddOrChangeCommand(Entity Target, Component Component)
        {
            this.Target = Target;
            this.Component = Component;
        }
        
        public override void OnSend(Player player)
        {
            AddOrChangeComponent();
        }

        public override void OnReceive(Player player)
        {
            AddOrChangeComponent();
        }

        // TODO: keep track of players entity shared with and broadcast any changes
        protected abstract void AddOrChangeComponent();

        [ProtocolFixed] public Entity Target { get; set; }
        [ProtocolFixed] public Component Component { get; set; }
    }
}