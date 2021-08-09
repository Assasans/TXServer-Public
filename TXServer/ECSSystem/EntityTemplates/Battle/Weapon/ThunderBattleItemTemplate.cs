using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-8770103861152493981L)]
	public class ThunderBattleItemTemplate : DiscreteWeaponTemplate
	{
        public static Entity CreateEntity(Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = CreateEntity(new ThunderBattleItemTemplate(), "garage/weapon/thunder", tank, battlePlayer);
            entity.Components.Add(new SplashImpactComponent(4f));
            entity.Components.Add(new SplashWeaponComponent(40f, 0f, 15f));
            entity.Components.Add(new ThunderComponent());

            return entity;
        }
    }
}
