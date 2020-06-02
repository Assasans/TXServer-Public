using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1544694405895L)]
    public class AvatarMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return AvatarUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
