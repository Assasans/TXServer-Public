using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Bonus
{
	[SerialVersionUID(5411677468097447480L)]
	public class SupplyBonusTemplate : BonusTemplate
	{
        public static Entity CreateEntity(BonusType bonusType, Entity bonusRegion, Vector3 position, Entity battleEntity) =>
            CreateEntity(new SupplyBonusTemplate(), $"battle/bonus/{bonusType.ToString().ToLower()}",
                bonusRegion, position, battleEntity);
    }
}
