using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1436443339132)]
    public class TankPaintMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry)
        {
            return new Entity(id, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/" + configPathEntry),
                new MarketItemGroupComponent(id));
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return TankPaintUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
