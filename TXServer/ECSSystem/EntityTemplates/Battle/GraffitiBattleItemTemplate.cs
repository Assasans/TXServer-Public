using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(636100801926133320L)]
    public class GraffitiBattleItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity userItem, Entity tank)
        {
            return new Entity(new TemplateAccessor(new GraffitiBattleItemTemplate(), userItem.TemplateAccessor.ConfigPath),
                new GraffitiBattleItemComponent(),
                tank.GetComponent<UserGroupComponent>(),
                userItem.GetComponent<MarketItemGroupComponent>());
        }
    }
}