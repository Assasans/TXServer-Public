using System;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(6)]
    public class ComponentChangeCommand : ComponentAddOrChangeCommand
    {
        public ComponentChangeCommand(Entity Target, Component Component) : base(Target, Component) { }

        protected override void AddOrChangeComponent()
        {
            if (!Target.Components.Remove(Component)) throw new ArgumentException("Component " + Component.GetType().FullName + " does not exist.");

            Target.Components.Add(Component);
        }
    }
}
