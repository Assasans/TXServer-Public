using System;
using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Bonus;
using TXServer.ECSSystem.Components.Battle.Location;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Bonus
{
	[SerialVersionUID(-8402520789413485183L)]
	public class GoldBonusTemplate : IEntityTemplate
	{
        public static Entity CreateEntity(Entity bonusRegion, Vector3 position, Entity battleEntity)
        {

            Entity entity = new Entity(new TemplateAccessor(new GoldBonusWithCrystalsTemplate(), "battle/bonus/gold/cry"),
                new BonusComponent(),
                new BonusDropTimeComponent(DateTime.UtcNow),
                new PositionComponent(position),
                new RotationComponent(new Vector3(0, 0, 0)),

                bonusRegion.GetComponent<BonusRegionGroupComponent>(),
                bonusRegion.GetComponent<GoldBonusRegionComponent>(),
                battleEntity.GetComponent<BattleGroupComponent>()
            );

            return entity;
        }
    }
}
