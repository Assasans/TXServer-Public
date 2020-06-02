using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1479807574456L)]
    public class ContainerUserItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(IEntityTemplate template, Entity marketItem, Entity user)
        {
            Entity item = new Entity(new TemplateAccessor(template, marketItem.TemplateAccessor.ConfigPath),
                new MarketItemGroupComponent(marketItem),
                new UserGroupComponent(user),
                new UserItemCounterComponent(1));
            item.Components.Add(new NotificationGroupComponent(item));

            return item;
        }

        public static Entity CreateEntity(Entity marketItem, Entity user) => CreateEntity(new ContainerUserItemTemplate(), marketItem, user);
    }
}
