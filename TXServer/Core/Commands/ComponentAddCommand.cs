using System;
using System.Reflection;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(4)]
    public class ComponentAddCommand : ComponentAddOrChangeCommand
    {
        public ComponentAddCommand(Entity Target, Component Component) : base(Target, Component) { }
        public ComponentAddCommand(Entity Target, Component Component, bool addOrChangeDone) : base(Target, Component, addOrChangeDone) { }

        public override void OnReceive(Player player)
        {
            base.OnReceive(player);

            MethodInfo method = Component.GetType().GetMethod("OnAttached", new[] { typeof(Player), typeof(Entity) });
            if (method != null)
            {
                method.Invoke(Component, new object[] { player, Target });
            }
        }

        protected override void AddOrChangeComponent() => Target.AddComponentLocally(Component);
    }
}
