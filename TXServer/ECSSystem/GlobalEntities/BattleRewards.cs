using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.BattleReward;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class BattleRewards
    {
        public static Items GlobalItems { get; } = new Items();

        public class Items : ItemList
        {
            public Entity Tutorial { get; } = new Entity(646862480, new TemplateAccessor(new TutorialBattleRewardTemplate(), "battle_rewards/tutorial"),
                new BattleRewardGroupComponent(646862480));
            public Entity NewLeague { get; } = new Entity(-1658996640, new TemplateAccessor(new LeagueFirstEntranceRewardTemplate(), "battle_rewards/new_league"),
                new BattleRewardGroupComponent(-1658996640));
            public Entity XCrystalBonus { get; } = new Entity(473122320, new TemplateAccessor(new XCrystalBattleRewardTemplate(), "battle_rewards/xcrystal_bonus"),
                new BattleRewardGroupComponent(473122320));
            public Entity ModuleContainer { get; } = new Entity(1069395872, new TemplateAccessor(new ModuleContainerBattleRewardTemplate(), "battle_rewards/module_container"),
                new BattleRewardGroupComponent(1069395872));
            public Entity LvlUpUnlock { get; } = new Entity(1672895928, new TemplateAccessor(new LevelUpUnlockBattleRewardTemplate(), "battle_rewards/lvlup_unlock"),
                new BattleRewardGroupComponent(1672895928));
        }
    }
}
