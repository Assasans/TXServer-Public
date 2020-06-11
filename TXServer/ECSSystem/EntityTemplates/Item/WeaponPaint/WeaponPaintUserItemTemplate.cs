using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636287154959625373L)]
    public class WeaponPaintUserItemTemplate : UserItemTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            Entity userItem = new Entity(new TemplateAccessor(new WeaponPaintUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath),
                new MarketItemGroupComponent(marketItem),
                new UserGroupComponent(user));

            AddToUserItems(typeof(Covers), userItem);
            return userItem;
        }
    }
}
