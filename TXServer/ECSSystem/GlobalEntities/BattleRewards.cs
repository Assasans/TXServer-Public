using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class BattleRewards
    {
        public static readonly Entity LvlUpUnlock = new Entity(1672895928, new TemplateAccessor(new LevelUpUnlockBattleRewardTemplate(), "battle_rewards/lvlup_unlock"), new BattleRewardGroupComponent(1672895928));
        public static readonly Entity ModuleContainer = new Entity(1069395872, new TemplateAccessor(new ModuleContainerBattleRewardTemplate(), "battle_rewards/module_container"), new BattleRewardGroupComponent(1069395872));
        public static readonly Entity NewLeague = new Entity(-1658996640, new TemplateAccessor(new LeagueFirstEntranceRewardTemplate(), "battle_rewards/new_league"), new BattleRewardGroupComponent(-1658996640));
        public static readonly Entity Tutorial = new Entity(646862480, new TemplateAccessor(new TutorialBattleRewardTemplate(), "battle_rewards/tutorial"), new BattleRewardGroupComponent(646862480));
        public static readonly Entity XCrystalBonus = new Entity(473122320, new TemplateAccessor(new XCrystalBattleRewardTemplate(), "battle_rewards/xcrystal_bonus"), new BattleRewardGroupComponent(473122320));
    }
}
