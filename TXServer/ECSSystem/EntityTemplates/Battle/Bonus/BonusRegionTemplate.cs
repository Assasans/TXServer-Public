using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Bonus;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Bonus
{
    [SerialVersionUID(8116072916726390829L)]
    public class BonusRegionTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(BonusType bonusType, Vector3 position)
        {
            Entity entity = new Entity(new TemplateAccessor(new BonusRegionTemplate(), "battle/bonus/region"),
                new BonusRegionComponent(bonusType),
                new SpatialGeometryComponent(position, new Vector3(0, 0, 0))
            );
            entity.Components.Add(new BonusRegionGroupComponent(entity));

            if (bonusType != BonusType.GOLD)
                entity.Components.Add(new SupplyBonusRegionComponent());
            else
                entity.Components.Add(new GoldBonusRegionComponent());

            return entity;
        }
    }
}
