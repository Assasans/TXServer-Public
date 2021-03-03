using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Bonus
{
    [SerialVersionUID(8566120830355322079L)]
    public class BonusRegionGroupComponent : GroupComponent
    {
        public BonusRegionGroupComponent(Entity entity) : base(entity)
        {
        }

        public BonusRegionGroupComponent(long Key) : base(Key)
        {
        }
    }
}