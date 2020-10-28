using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1480333679186)]
    public class BattlePingResultEvent : ECSEvent
    {
        public float ClientSendRealTime { get; set; }
        
        public float ClientReceiveRealTime { get; set; }

        public void Execute(Player player, Entity entity)
        {
            //todo
        }
    }
}