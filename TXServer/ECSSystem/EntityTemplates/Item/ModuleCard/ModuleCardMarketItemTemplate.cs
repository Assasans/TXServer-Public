using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636319307214133884L)]
    public class ModuleCardMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return ModuleCardUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
