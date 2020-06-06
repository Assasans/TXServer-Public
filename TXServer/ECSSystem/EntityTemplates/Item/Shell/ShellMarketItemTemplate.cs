using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(716181447780635764)]
    public class ShellMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry, Entity parent)
        {
            return new Entity(id, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/" + configPathEntry),
                new ParentGroupComponent(parent),
                new MarketItemGroupComponent(id));
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return ShellUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
