using System;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(4)]
    public class ComponentAddCommand : ComponentAddOrChangeCommand
    {
        public ComponentAddCommand(Entity Target, Component Component) : base(Target, Component) { }

        protected override void AddOrChangeComponent()
        {
            if (!Target.Components.Add(Component))
                throw new ArgumentException("Компонент " + Component.GetType().FullName + " уже содержится в сущности.");
        }
    }
}
