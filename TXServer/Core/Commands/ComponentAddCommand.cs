using System;
using TXServer.Core.ECSSystem;
using TXServer.Core.ECSSystem.Components;

namespace TXServer.Core.Commands
{
    public class ComponentAddCommand : ComponentAddOrChangeCommand
    {
        public ComponentAddCommand(Entity Target, Component Component) : base(Target, Component) { }

        protected override void AddOrChangeComponent()
        {
            if (Target.Components.ContainsKey(Component.GetType()))
                throw new ArgumentException("Компонент " + Component.GetType().FullName + " уже содержится в сущности.");
            Target.Components.TryAdd(Component.GetType(), Component);
        }
    }
}
