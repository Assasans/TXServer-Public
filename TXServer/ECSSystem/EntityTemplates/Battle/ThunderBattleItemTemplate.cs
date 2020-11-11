using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-8770103861152493981L)]
	public class ThunderBattleItemTemplate : DiscreteWeaponTemplate
	{
        public static Entity CreateEntity(Entity tank)
        {
            Entity entity = CreateEntity(new ThunderBattleItemTemplate(), "battle/weapon/thunder", tank);
            entity.Components.Add(new SplashImpactComponent(4f));
            entity.Components.Add(new SplashWeaponComponent(40f, 0f, 15f));
            entity.Components.Add(new ThunderComponent());

            return entity;
        }
    }
}