using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components.Battle.Bonus
{
    [SerialVersionUID(-3961778961585441606L)]
    public class BonusRegionComponent : Component
    {

        public BonusRegionComponent(BonusType type)
        {
            Type = type;
        }

        public BonusType Type { get; set; }
    }
}