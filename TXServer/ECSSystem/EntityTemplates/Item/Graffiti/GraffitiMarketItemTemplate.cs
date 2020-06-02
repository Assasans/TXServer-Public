using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636100801770520539L)]
    public class GraffitiMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return GraffitiUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
