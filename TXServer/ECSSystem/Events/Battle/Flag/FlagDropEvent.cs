using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(2921314315544889042L)]
    public class FlagDropEvent : ECSEvent
    {
        public FlagDropEvent(bool IsUserAction)
        {
            this.IsUserAction = IsUserAction;
        }

        public bool IsUserAction { get; set; }
    }
}