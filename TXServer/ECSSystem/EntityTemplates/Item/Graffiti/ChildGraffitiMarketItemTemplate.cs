using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636100801497439942L)]
    public class ChildGraffitiMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return GraffitiUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
