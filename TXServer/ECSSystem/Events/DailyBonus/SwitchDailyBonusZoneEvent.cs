using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.DailyBonus
{
    [SerialVersionUID(636461688330473034L)]
    public class SwitchDailyBonusZoneEvent : ECSEvent
    {
        public void Execute(Player player, Entity user) => player.Data.DailyBonusZone++;
    }
}
