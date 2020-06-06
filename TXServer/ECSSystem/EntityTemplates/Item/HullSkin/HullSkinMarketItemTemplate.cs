using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1469607967377L)]
    public class HullSkinMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry, Entity parent, bool isDefault = false)
        {
            Entity userItem = new Entity(id, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/" + configPathEntry),
                new ParentGroupComponent(parent),
                new MarketItemGroupComponent(id));

            if (isDefault)
                userItem.Components.Add(new DefaultSkinItemComponent());

            return userItem;
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return HullSkinUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
