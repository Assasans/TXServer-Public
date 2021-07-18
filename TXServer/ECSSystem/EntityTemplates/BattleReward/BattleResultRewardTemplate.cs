using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.BattleRewards;

namespace TXServer.ECSSystem.EntityTemplates.BattleReward
{
    [SerialVersionUID(1513235522063L)]
    public class BattleResultRewardTemplate : IEntityTemplate
    {
        protected static Entity CreateEntity(BattleResultRewardTemplate template, string configPath)
        {
            Entity battleReward = new(new TemplateAccessor(template, configPath),
                new PersonalBattleRewardComponent());

            battleReward.AddComponent(new BattleRewardGroupComponent(battleReward));

            return battleReward;
        }
    }
}
