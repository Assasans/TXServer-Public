using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636100801770520539L)]
    public class GraffitiMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry)
        {
            return new Entity(id, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/" + configPathEntry),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(id));
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return GraffitiUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
