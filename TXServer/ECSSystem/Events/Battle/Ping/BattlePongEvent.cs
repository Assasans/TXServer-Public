using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Ping
{
    [SerialVersionUID(1480333153972)]
    public class BattlePongEvent : ECSEvent
    {
        public BattlePongEvent(float clientSendRealTime)
        {
            ClientSendRealTime = clientSendRealTime;
        }

        public float ClientSendRealTime { get; set; }
    }
}
