using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1469607756132L)]
    public class WeaponSkinUserItemTemplate : UserItemTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            Entity userItem = new Entity(new TemplateAccessor(new WeaponSkinUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath),
                marketItem.GetComponent<ParentGroupComponent>(),
                new MarketItemGroupComponent(marketItem),
                new UserGroupComponent(user));

            AddToUserItems(typeof(WeaponSkins), userItem);

            return userItem;
        }
    }
}
