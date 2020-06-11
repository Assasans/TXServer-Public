using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1479807574456L)]
    public class ContainerUserItemTemplate : UserItemTemplate, ICountableItemTemplate
    {
        public static Entity CreateEntity(IEntityTemplate template, Entity marketItem, Entity user)
        {
            Entity userItem = GetExistingUserItem(typeof(Containers), marketItem);

            if (userItem == null)
            {
                userItem = new Entity(new TemplateAccessor(template, marketItem.TemplateAccessor.ConfigPath),
                    new MarketItemGroupComponent(marketItem),
                    new UserGroupComponent(user),
                    new UserItemCounterComponent(0));
                userItem.Components.Add(new NotificationGroupComponent(userItem));

                AddToUserItems(typeof(Containers), userItem);
            }

            return userItem;
        }

        public static Entity CreateEntity(Entity marketItem, Entity user) => CreateEntity(new ContainerUserItemTemplate(), marketItem, user);
    }
}
