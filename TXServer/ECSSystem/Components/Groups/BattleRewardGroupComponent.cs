using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1513677547945L)]
    public class BattleRewardGroupComponent : GroupComponent
    {
        public BattleRewardGroupComponent(Entity entity) : base(entity)
        {
        }

        public BattleRewardGroupComponent(long Key) : base(Key)
        {
        }
    }
}
