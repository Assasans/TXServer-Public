using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636100801716991373L)]
    public class GraffitiUserItemTemplate : IEntityTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            return new Entity(new TemplateAccessor(new GraffitiUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath),
                new MarketItemGroupComponent(marketItem),
                new UserGroupComponent(user));
        }
    }
}
