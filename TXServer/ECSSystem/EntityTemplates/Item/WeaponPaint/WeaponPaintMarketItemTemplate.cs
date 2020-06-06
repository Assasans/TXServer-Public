using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636287153836461132L)]
    public class WeaponPaintMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry)
        {
            return new Entity(id, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/" + configPathEntry),
                new MarketItemGroupComponent(id));
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return WeaponPaintUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
