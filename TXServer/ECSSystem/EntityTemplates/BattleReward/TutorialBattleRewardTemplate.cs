using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.BattleReward
{
    [SerialVersionUID(1514023810287L)]
    public class TutorialBattleRewardTemplate : BattleResultRewardTemplate
    {
        public static Entity CreateEntity() =>
            CreateEntity(new TutorialBattleRewardTemplate(), "battle_rewards/tutorial");
    }
}
