using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636413290399070700L)]
	public class TutorialGameplayChestMarketItemTemplate : IMarketItemTemplate
	{
        public static Entity CreateEntity(long id, string configPathEntry)
        {
            return new Entity(id, new TemplateAccessor(new TutorialGameplayChestMarketItemTemplate(), "garage/container/" + configPathEntry),
                new MarketItemGroupComponent(id));
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return TutorialGameplayChestUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
