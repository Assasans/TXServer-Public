using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(636287143924344191)]
    public class WeaponPaintBattleItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity userItem, Entity tank)
        {
            return new Entity(new TemplateAccessor(new WeaponPaintBattleItemTemplate(), userItem.TemplateAccessor.ConfigPath),
                new WeaponPaintBattleItemComponent(),
                tank.GetComponent<UserGroupComponent>(),
                tank.GetComponent<BattleGroupComponent>(),
                tank.GetComponent<TankGroupComponent>(),
                userItem.GetComponent<MarketItemGroupComponent>());
        }
    }
}