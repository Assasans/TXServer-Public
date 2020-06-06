using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636100801497439942L)]
    public class ChildGraffitiMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry, Entity parent)
        {
            return new Entity(id, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/" + configPathEntry),
                new RestrictionByUserFractionComponent(),
                new ParentGroupComponent(parent),
                new MarketItemGroupComponent(id));

        }
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return GraffitiUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
