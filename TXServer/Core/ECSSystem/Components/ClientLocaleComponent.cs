using System.IO;

namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1453796862447)]
    public sealed class ClientLocaleComponent : Component
    {
        public ClientLocaleComponent() { }

        public ClientLocaleComponent(BinaryReader reader) => Unwrap(reader);

        public override void Unwrap(BinaryReader reader)
        {
            LocaleCode = reader.ReadString();
        }

        [Protocol] public string LocaleCode { get; set; }
    }
}
