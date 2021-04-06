using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(4)]
    public class ComponentAddCommand : ComponentAddOrChangeCommand
    {
        public ComponentAddCommand(Entity Target, Component Component) : base(Target, Component) { }

        public override void OnReceive(Player player)
        {
            base.OnReceive(player);

            Component.GetType()
                     .GetMethod("OnAttached", new[] { typeof(Player), typeof(Entity) })
                     ?.Invoke(Component, new object[] { player, Target });
        }

        protected override void AddOrChangeComponent(Player player) => Target.AddComponent(Component, player);
    }
}
