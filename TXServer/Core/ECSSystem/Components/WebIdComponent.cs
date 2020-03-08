using System.IO;
using TXServer.Bits;
using TXServer.Core.ECSSystem;

namespace TXServer.Core.ECSSystem
{
    public static partial class Components
    {
        [SerialVersionUID(1479820450460)]
        public sealed class WebIdComponent : Component
        {
            public WebIdComponent() { }

            public override void Wrap(BinaryWriter writer)
            {
                writer.Write(WebId);
                writer.Write(Utm);
                writer.Write(GoogleAnalyticsId);
                writer.Write(WebIdUid);
            }

            public override void Unwrap(BinaryReader reader)
            {
                WebId = reader.ReadString();
                Utm = reader.ReadString();
                GoogleAnalyticsId = reader.ReadString();
                WebIdUid = reader.ReadString();
            }

            [Protocol] public string WebId { get; set; } = "";
            [Protocol] public string Utm { get; set; } = "";
            [Protocol] public string GoogleAnalyticsId { get; set; } = "";
            [Protocol] public string WebIdUid { get; set; } = "";
        }
    }
}