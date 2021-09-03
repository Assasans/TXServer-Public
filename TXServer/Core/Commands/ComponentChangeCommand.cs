using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(6)]
    public class ComponentChangeCommand : ComponentAddOrChangeCommand
    {
        public ComponentChangeCommand(Entity Target, Component Component) : base(Target, Component) { }

        public override void OnReceive(Player player)
        {
            base.OnReceive(player);

            Component.GetType()
                .GetMethod("OnChanged", new[] { typeof(Player), typeof(Entity) })
                ?.Invoke(Component, new object[] { player, Target });
        }

        protected override void AddOrChangeComponent(Player player) => Target.ChangeComponent(Component, player);
    }
}
