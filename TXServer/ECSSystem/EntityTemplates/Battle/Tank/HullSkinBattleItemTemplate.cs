using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Hull;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(636047163591561471)]
    public class HullSkinBattleItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity userItem, Entity tank)
        {
            return new Entity(new TemplateAccessor(new HullSkinBattleItemTemplate(), userItem.TemplateAccessor.ConfigPath),
                new HullSkinBattleItemComponent(),
                tank.GetComponent<UserGroupComponent>(),
                tank.GetComponent<BattleGroupComponent>(),
                tank.GetComponent<TankGroupComponent>(),
                userItem.GetComponent<MarketItemGroupComponent>());
        }
    }
}