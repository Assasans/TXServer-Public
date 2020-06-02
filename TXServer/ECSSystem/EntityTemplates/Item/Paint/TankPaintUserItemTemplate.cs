using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1438603647557L)]
    public class TankPaintUserItemTemplate : IEntityTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            Entity item = new Entity(new TemplateAccessor(new TankPaintUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath), marketItem.Components);
            item.Components.Add(new UserGroupComponent(user));

            return item;
        }
    }
}
