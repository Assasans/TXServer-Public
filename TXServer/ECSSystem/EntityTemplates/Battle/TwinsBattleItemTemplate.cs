using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(583528765588657091L)]
	public class TwinsBattleItemTemplate : DiscreteWeaponTemplate
	{
        public static Entity CreateEntity(Entity tank)
        {
            Entity entity = CreateEntity(new TwinsBattleItemTemplate(), "battle/weapon/twins", tank);
            entity.Components.Add(new WeaponBulletShotComponent(0.5f, 150f));
            entity.Components.Add(new TwinsComponent());

            return entity;
        }
    }
}