using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1469607958560L)]
    public class HullSkinUserItemTemplate : IEntityTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            return new Entity(new TemplateAccessor(new HullSkinUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath),
                marketItem.GetComponent<ParentGroupComponent>(),
                new MarketItemGroupComponent(marketItem),
                new UserGroupComponent(user));
        }
    }
}
