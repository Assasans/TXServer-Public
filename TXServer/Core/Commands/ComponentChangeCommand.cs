using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(6)]
    public class ComponentChangeCommand : ComponentAddOrChangeCommand
    {
        public ComponentChangeCommand(Entity Target, Component Component) : base(Target, Component) { }

        protected override void AddOrChangeComponent(Player player) => Target.ChangeComponent(Component, player);
    }
}
