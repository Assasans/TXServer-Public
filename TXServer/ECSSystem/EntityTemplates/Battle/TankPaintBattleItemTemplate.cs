using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1437375358285)]
    public class TankPaintBattleItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity userItem, Entity tank)
        {
            return new Entity(new TemplateAccessor(new TankPaintBattleItemTemplate(), userItem.TemplateAccessor.ConfigPath),
                new TankPaintBattleItemComponent(),
                tank.GetComponent<UserGroupComponent>(),
                tank.GetComponent<BattleGroupComponent>(),
                tank.GetComponent<TankGroupComponent>(),
                userItem.GetComponent<MarketItemGroupComponent>());
        }
    }
}