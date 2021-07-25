using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.DailyBonus
{
    [SerialVersionUID(636459062089487176)]
    public class UserDailyBonusZoneComponent : Component
    {
        public UserDailyBonusZoneComponent(Player player)
        {
            ZoneNumber = player.Data.DailyBonusZone;
            SelfOnlyPlayer = player;
        }

        public long ZoneNumber { get; set; }
    }
}
