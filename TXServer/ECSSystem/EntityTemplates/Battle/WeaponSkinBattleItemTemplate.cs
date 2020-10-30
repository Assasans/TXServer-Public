using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(636046254605033322)]
    public class WeaponSkinBattleItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity userItem, Entity tank)
        {
            return new Entity(new TemplateAccessor(new WeaponSkinBattleItemTemplate(), userItem.TemplateAccessor.ConfigPath),
                new WeaponSkinBattleItemComponent(),
                tank.GetComponent<UserGroupComponent>(),
                tank.GetComponent<BattleGroupComponent>(),
                tank.GetComponent<TankGroupComponent>(),
                userItem.GetComponent<MarketItemGroupComponent>());
        }
    }
}