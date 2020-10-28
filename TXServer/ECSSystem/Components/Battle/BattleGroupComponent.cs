using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(1140613249019529884)]
    public class BattleGroupComponent : GroupComponent
    {
        public BattleGroupComponent(Entity entity) : base(entity)
        {
        }

        public BattleGroupComponent(long Key) : base(Key)
        {
        }
    }
}