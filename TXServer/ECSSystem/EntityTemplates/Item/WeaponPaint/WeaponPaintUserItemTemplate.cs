using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636287154959625373L)]
    public class WeaponPaintUserItemTemplate : IEntityTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            return new Entity(new TemplateAccessor(new WeaponPaintUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath),
                new MarketItemGroupComponent(marketItem),
                new UserGroupComponent(user));
        }
    }
}
