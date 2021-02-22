using System;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(6)]
    public class ComponentChangeCommand : ComponentAddOrChangeCommand
    {
        public ComponentChangeCommand(Entity Target, Component Component, bool addOrChangeDone = false) : base(Target, Component, addOrChangeDone) { }

        protected override void AddOrChangeComponent()
        {
            Target.ChangeComponentLocally(Component);
        }
    }
}
