using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1438603647557L)]
    public class TankPaintUserItemTemplate : UserItemTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            Entity userItem = new Entity(new TemplateAccessor(new TankPaintUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath), marketItem.Components);
            userItem.Components.Add(new UserGroupComponent(user));

            AddToUserItems(typeof(Paints), userItem);
            return userItem;
        }
    }
}
