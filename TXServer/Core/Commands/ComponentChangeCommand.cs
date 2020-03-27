using System;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    public class ComponentChangeCommand : ComponentAddOrChangeCommand
    {
        public ComponentChangeCommand() { }

        public ComponentChangeCommand(Entity Target, Component Component) : base(Target, Component) { }

        protected override void AddOrChangeComponent()
        {
            if (!Target.Components.Contains(Component)) throw new ArgumentException("Компонент " + Component.GetType().FullName + " не найден.");

            Target.Components.Remove(Component);
            Target.Components.Add(Component);
        }
    }
}
