using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636100801716991373L)]
    public class GraffitiUserItemTemplate : UserItemTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            Entity userItem = new Entity(new TemplateAccessor(new GraffitiUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath),
                new MarketItemGroupComponent(marketItem),
                new UserGroupComponent(user));

            AddToUserItems(typeof(Graffiti), userItem);
            return userItem;
        }
    }
}
