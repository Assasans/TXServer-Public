using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Flag
{
    [SerialVersionUID(2921314315544889042L)]
    public class FlagDropEvent : ECSEvent
    {
        public FlagDropEvent(bool isUserAction)
        {
            IsUserAction = isUserAction;
        }

        public bool IsUserAction { get; set; }
    }
}
