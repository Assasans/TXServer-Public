using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Matchmaking
{
    [SerialVersionUID(1509109822442)]
    public class ExitedFromMatchMakingEvent : ECSEvent
    {
        public ExitedFromMatchMakingEvent(bool selfAction)
        {
            SelfAction = selfAction;
        }
        
        public bool SelfAction { get; set; }
    }
}