using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636408122917164205L)]
    public class DonutChestMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry)
        {
            return new Entity(id, new TemplateAccessor(new DonutChestMarketItemTemplate(), "garage/container/" + configPathEntry),
                new MarketItemGroupComponent(id));
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return SimpleChestUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
