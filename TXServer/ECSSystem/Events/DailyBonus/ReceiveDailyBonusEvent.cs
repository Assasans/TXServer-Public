using System;
using TXServer.Core;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.DailyBonus;

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

            if (player.User.GetComponent<UserStatisticsComponent>().Statistics["BATTLES_PARTICIPATED"] <
                bonusConfig.BattleCountToUnlockDailyBonuses)
                return;

            player.Data.AddDailyBonusReward(Code);
            player.Data.DailyBonusNextReceiveDate =
                DateTime.UtcNow.AddSeconds(bonusConfig.ReceivingBonusIntervalInSeconds /
                                           (player.Data.IsPremium ? bonusConfig.PremiumTimeSpeedUp : 1.0));

            player.SendEvent(new DailyBonusReceivedEvent(Code), player.User);
        }

        public long Code { get; set; }
    }
}
