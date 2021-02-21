using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1475648914994L)]
    public class CompleteBuyUIDChangeEvent : ECSEvent
    {
        public CompleteBuyUIDChangeEvent(bool success)
        {
            this.Success = success;
        }

        public bool Success { get; set; }
    }
}
