using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1469607958560L)]
    public class HullSkinUserItemTemplate : UserItemTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            Entity userItem = new Entity(new TemplateAccessor(new HullSkinUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath),
                marketItem.GetComponent<ParentGroupComponent>(),
                new MarketItemGroupComponent(marketItem),
                new UserGroupComponent(user));

            AddToUserItems(typeof(HullSkins), userItem);
            return userItem;
        }
    }
}
