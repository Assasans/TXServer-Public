using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1469607574709L)]
    public class WeaponSkinMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry, Entity parent, bool isDefault = false)
        {
            Entity userItem = new Entity(id, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/" + configPathEntry),
                new ParentGroupComponent(parent),
                new MarketItemGroupComponent(id));

            if (isDefault)
                userItem.Components.Add(new DefaultSkinItemComponent());

            return userItem;
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return WeaponSkinUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
