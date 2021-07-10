using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.DailyBonus
{
    [SerialVersionUID(636462569803130386L)]
    public class DailyBonusCommonConfigComponent : Component
    {
        public long ReceivingBonusIntervalInSeconds { get; set; }
        public long BattleCountToUnlockDailyBonuses { get; set; }
        public int PremiumTimeSpeedUp { get; set; }
    }
}
