using System.IO;

namespace TXServer.Core.ECSSystem.Events
{
    [SerialVersionUID(1453867134827)]
    public class EnterScreenEvent : ECSEvent
    {
        public EnterScreenEvent() { }

        public EnterScreenEvent(BinaryReader reader)
        {
            Screen = reader.ReadString();
        }

        [Protocol] public string Screen { get; set; }

        public override void Execute(Entity entity) { }
    }
}
