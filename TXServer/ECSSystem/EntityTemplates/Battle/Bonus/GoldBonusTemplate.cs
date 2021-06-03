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
	public class GoldBonusTemplate : BonusTemplate
	{
        public static Entity CreateEntity(Entity bonusRegion, Vector3 position, Entity battleEntity)
        {
            Entity bonus = CreateEntity(new GoldBonusTemplate(), "battle/bonus/gold/cry", bonusRegion,
                position, battleEntity);
            bonus.AddComponent(bonusRegion.GetComponent<GoldBonusRegionComponent>());

            return bonus;
        }
    }
}
