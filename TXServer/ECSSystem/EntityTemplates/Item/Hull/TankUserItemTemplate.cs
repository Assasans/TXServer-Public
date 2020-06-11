using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1438603503434L)]
    public class TankUserItemTemplate : UserItemTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            Entity userItem = new Entity(new TemplateAccessor(new TankUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath),
                new ParentGroupComponent(marketItem),
                new MarketItemGroupComponent(marketItem),
                new UserGroupComponent(user),
                new ExperienceItemComponent(),
                new UpgradeLevelItemComponent(),
                new UpgradeMaxLevelItemComponent());

            AddToUserItems(typeof(Hulls), userItem);
            return userItem;
        }
    }
}
