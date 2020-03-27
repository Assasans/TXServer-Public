using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1453867134827)]
    public class EnterScreenEvent : ECSEvent
    {
        public override void Execute(Entity entity) { }

        public string Screen { get; set; }
    }
}
