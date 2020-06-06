using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1433406732656L)]
    public class TankMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry)
        {
            return new Entity(id, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/" + configPathEntry),
                new ParentGroupComponent(id),
                new MarketItemGroupComponent(id));
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return TankUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
