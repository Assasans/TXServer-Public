using System;
using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Bonus;
using TXServer.ECSSystem.Components.Battle.Location;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Bonus
{
	[SerialVersionUID(-8402520789413485183L)]
	public class GoldBonusTemplate : BonusTemplate, IEntityTemplate
	{
        public static Entity CreateEntity(BonusType bonusType, Entity bonusRegion, Vector3 position, Entity battleEntity)
        {

            Entity entity = new Entity(new TemplateAccessor(new GoldBonusWithCrystalsTemplate(), "battle/bonus/gold/cry"),
                new BonusComponent(),
                new BonusDropTimeComponent(DateTime.UtcNow.AddMinutes(1)),
                new PositionComponent(position),
                new RotationComponent(new Vector3(0, 0, 0)),
                new BonusRegionGroupComponent(bonusRegion),
                new BattleGroupComponent(battleEntity)
            );

            return entity;
        }
    }
}
