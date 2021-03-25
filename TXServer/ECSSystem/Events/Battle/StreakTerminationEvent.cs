using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1512395506558L)]
    public class StreakTerminationEvent : ECSEvent
    {
        public StreakTerminationEvent(string victimUid)
        {
            VictimUid = victimUid;
        }
        public string VictimUid { get; set; }
    }
}