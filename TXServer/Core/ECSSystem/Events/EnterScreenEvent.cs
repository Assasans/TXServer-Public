using System.IO;

namespace TXServer.Core.ECSSystem.Events
{
    [SerialVersionUID(1453867134827)]
    public class EnterScreenEvent : ECSEvent
    {
        public override void Execute(Entity entity) { }

        [Protocol] public string Screen { get; set; }
    }
}
