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
            if (!Target.Components.Remove(Component)) throw new ArgumentException("Компонент " + Component.GetType().FullName + " не найден.");

            Target.Components.Add(Component);
        }
    }
}
