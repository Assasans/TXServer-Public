using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1438070264777L)]
    public class SaveAutoLoginTokenEvent : ECSEvent
    {
        public string Uid { get; set; }
        public byte[] Token { get; set; }
    }
}
