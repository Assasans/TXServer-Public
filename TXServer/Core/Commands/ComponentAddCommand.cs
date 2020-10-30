using System;
using System.Reflection;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(4)]
    public class ComponentAddCommand : ComponentAddOrChangeCommand
    {
        public ComponentAddCommand(Entity Target, Component Component) : base(Target, Component) { }

        protected override void AddOrChangeComponent(Player player)
        {
            if (!Target.Components.Add(Component))
                throw new ArgumentException("Entity already contains component" + Component.GetType().FullName + ".");

            MethodInfo method = Component.GetType().GetMethod("OnAttached", new[] { typeof(Player), typeof(Entity) });
            if (method != null)
            {
                method.Invoke(Component, new object[] { player, Target });
            }
        }
    }
}
