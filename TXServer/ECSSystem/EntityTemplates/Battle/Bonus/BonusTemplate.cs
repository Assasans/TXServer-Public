using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Bonus;
using TXServer.ECSSystem.Types;
using System;
using System.Numerics;
using TXServer.ECSSystem.Components.Battle.Location;
using TXServer.ECSSystem.Components.Battle;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Bonus
{
    [SerialVersionUID(7553964914512142106L)]
    public class BonusTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(BonusType bonusType, Entity bonusRegion, Vector3 position, Entity battleEntity)
        {
            Entity entity = new Entity(new TemplateAccessor(new SupplyBonusTemplate(), "battle/bonus/" + bonusType.ToString().ToLower()),
                new BonusComponent(),
                new BonusDropTimeComponent(DateTime.UtcNow),
                new PositionComponent(position),
                new RotationComponent(new Vector3(0, 0, 0)),
                new BonusRegionGroupComponent(bonusRegion),
                new BattleGroupComponent(battleEntity)
            );

            return entity;
        }
    }
}
