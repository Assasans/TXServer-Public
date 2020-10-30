using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(-5342270968507348251)]
    public class ShellBattleItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity userItem, Entity tank)
        {
            return new Entity(new TemplateAccessor(new ShellBattleItemTemplate(), userItem.TemplateAccessor.ConfigPath),
                new ShellBattleItemComponent(),
                tank.GetComponent<UserGroupComponent>(),
                tank.GetComponent<BattleGroupComponent>(),
                tank.GetComponent<TankGroupComponent>(),
                userItem.GetComponent<MarketItemGroupComponent>());
        }
    }
}