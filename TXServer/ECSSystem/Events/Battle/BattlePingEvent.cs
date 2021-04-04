using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1480326022618)]
    public class BattlePingEvent : ECSEvent
    {
        public float ClientSendRealTime { get; set; }

        public void Execute(Player player, Entity entity)
        {
            player.SendEvent(new BattlePongEvent(ClientSendRealTime), entity);
        }
    }
}