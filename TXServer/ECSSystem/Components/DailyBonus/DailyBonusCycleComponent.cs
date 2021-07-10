using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.DailyBonus
{
    public class DailyBonusCycleComponent : Component
    {
        public int[] Zones { get; set; }
        public DailyBonusData[] DailyBonuses { get; set; }
    }

    public class DailyBonusData
    {
        public long Code { get; set; }
        public long CryAmount { get; set; }
        public long XcryAmount { get; set; }
        public long EnergyAmount { get; set; }
        public DailyBonusGarageItemReward ContainerReward { get; set; }
        public DailyBonusGarageItemReward DetailReward { get; set; }
    }

    public class DailyBonusGarageItemReward
    {
        public long MarketItemId { get; set; }
        public long Amount { get; set; }
    }
}
