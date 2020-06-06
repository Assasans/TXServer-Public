using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    public abstract class WeaponMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry, IEntityTemplate template)
        {
            return new Entity(id, new TemplateAccessor(template, "garage/weapon/" + configPathEntry),
                new ParentGroupComponent(id),
                new MarketItemGroupComponent(id));
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return WeaponUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
