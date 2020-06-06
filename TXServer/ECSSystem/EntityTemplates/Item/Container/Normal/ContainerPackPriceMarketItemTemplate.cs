using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1489474099632)]
    public class ContainerPackPriceMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry)
        {
            return new Entity(id, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/" + configPathEntry),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(id));
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return ContainerUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
