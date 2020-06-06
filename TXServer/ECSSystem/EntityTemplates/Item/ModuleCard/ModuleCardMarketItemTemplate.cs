using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636319307214133884L)]
    public class ModuleCardMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry, Entity parent)
        {
            return new Entity(id, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/" + configPathEntry),
                new ParentGroupComponent(parent),
                new MarketItemGroupComponent(id));
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return ModuleCardUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
