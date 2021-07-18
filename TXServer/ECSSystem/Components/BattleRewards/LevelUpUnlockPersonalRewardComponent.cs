using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.BattleRewards
{
    [SerialVersionUID(1514202494334L)]
    public class LevelUpUnlockPersonalRewardComponent : Component
    {
        public LevelUpUnlockPersonalRewardComponent(List<Entity> unlocked)
        {
            Unlocked = unlocked;
        }

        public List<Entity> Unlocked { get; set; }
    }
}
