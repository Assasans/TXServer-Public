using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.DailyBonus
{
    [SerialVersionUID(636459062089487176)]
    public class UserDailyBonusZoneComponent : Component
    {
        public UserDailyBonusZoneComponent(long ZoneNumber)
        {
            this.ZoneNumber = ZoneNumber;
        }

        public long ZoneNumber { get; set; }
    }
}
