using System;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.DailyBonus;
using ResourceManager = TXServer.ECSSystem.GlobalEntities.ResourceManager;

namespace TXServer.ECSSystem.Events.DailyBonus
{
    [SerialVersionUID(636458159324341964L)]
    public class ReceiveDailyBonusEvent : ECSEvent
    {
        private const string ConfigPath = "dailybonus";

        public void Execute(Player player, Entity clientSession)
        {
            DailyBonusCommonConfigComponent bonusConfig =
                Config.GetComponent<DailyBonusCommonConfigComponent>(ConfigPath);
            DailyBonusFirstCycleComponent firstCycleComponent =
                Config.GetComponent<DailyBonusFirstCycleComponent>(ConfigPath);
            DailyBonusEndlessCycleComponent endlessCycleComponent =
                Config.GetComponent<DailyBonusEndlessCycleComponent>(ConfigPath);
            DailyBonusCycleComponent cycleComponent =
                player.Data.DailyBonusCycle > 0 ? endlessCycleComponent : firstCycleComponent;

            if (player.Data.Statistics.BattlesParticipated < bonusConfig.BattleCountToUnlockDailyBonuses)
                return;

            // collect reward
            DailyBonusData reward = cycleComponent.DailyBonuses.Single(db => db.Code == Code);
            player.Data.Crystals += reward.CryAmount;
            player.Data.XCrystals += reward.XcryAmount;
            if (reward.ContainerReward != null)
            {
                Entity marketItem = player.EntityList.Single(e => e.EntityId == reward.ContainerReward.MarketItemId);
                ResourceManager.SaveNewMarketItem(player, marketItem, (int) reward.ContainerReward.Amount);
            }
            if (reward.DetailReward != null)
            {
                Entity marketItem = player.EntityList.Single(e => e.EntityId == reward.DetailReward.MarketItemId);
                ResourceManager.SaveNewMarketItem(player, marketItem, (int) reward.DetailReward.Amount);
            }

            // set next daily bonus
            player.Data.AddDailyBonusReward(Code);
            player.Data.DailyBonusNextReceiveDate =
                DateTime.UtcNow.AddSeconds(bonusConfig.ReceivingBonusIntervalInSeconds /
                                           (player.Data.IsPremium ? bonusConfig.PremiumTimeSpeedUp : 1.0));

            player.SendEvent(new DailyBonusReceivedEvent(Code), player.User);
        }

        public long Code { get; set; }
    }
}
