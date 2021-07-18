using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.BattleRewards;

namespace TXServer.ECSSystem.EntityTemplates.BattleReward
{
    [SerialVersionUID(1514196284686L)]
    public class LevelUpUnlockBattleRewardTemplate : BattleResultRewardTemplate
    {
        public static Entity CreateEntity(List<Entity> unlockedItems)
        {
            Entity battleReward = CreateEntity(new LevelUpUnlockBattleRewardTemplate(), "battle_rewards/lvlup_unlock");

            battleReward.AddComponent(new LevelUpUnlockPersonalRewardComponent(unlockedItems));

            return battleReward;
        }
    }
}
