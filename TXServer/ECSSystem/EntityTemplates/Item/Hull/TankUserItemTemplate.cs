using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1438603503434L)]
    public class TankUserItemTemplate : IEntityTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            return new Entity(new TemplateAccessor(new TankUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath),
                new ParentGroupComponent(marketItem),
                new MarketItemGroupComponent(marketItem),
                new UserGroupComponent(user),
                new ExperienceItemComponent(),
                new UpgradeLevelItemComponent(),
                new UpgradeMaxLevelItemComponent());
        }
    }
}
