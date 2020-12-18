using System.Linq;
using System.Threading;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    public abstract class ComponentAddOrChangeCommand : ComponentCommand
    {
        public ComponentAddOrChangeCommand(Entity Target, Component Component)
        {
            this.Target = Target;
            this.Component = Component;
        }

        public override bool OnSend(Player player)
        {
            AddOrChangeComponent(player);
            return true;
        }

        public override void OnReceive(Player player)
        {
            AddOrChangeComponent(player);
        }

        private void AddOrChangeComponent(Player player)
        {
            if (!MarkAsCompleted()) return;
            lock (Target.PlayerReferences)
                CommandManager.BroadcastCommands(Target.PlayerReferences.Keys.Where(x => x != player), this);
            AddOrChangeComponent();
        }

        private bool MarkAsCompleted()
        {
            return Interlocked.Exchange(ref addOrChangeDone, 1) == 0;
        }

        protected abstract void AddOrChangeComponent();

        [ProtocolFixed] public Entity Target { get; set; }
        [ProtocolFixed] public Component Component { get; set; }

        private int addOrChangeDone;
    }
}