using System;
using TXServer.Core.ECSSystem;
using static TXServer.Core.ECSSystem.Components;

namespace TXServer.Core.Commands
{
    public class ComponentChangeCommand : ComponentAddOrChangeCommand
    {
        public ComponentChangeCommand() { }

        public ComponentChangeCommand(Entity Target, Component Component) : base(Target, Component) { }

        protected override void AddOrChangeComponent()
        {
            if (!Target.Components.ContainsKey(Component.GetType())) throw new ArgumentException("Компонент " + Component.GetType().FullName + " не найден.");
            Target.Components[Component.GetType()] = Component;
        }
    }
}
