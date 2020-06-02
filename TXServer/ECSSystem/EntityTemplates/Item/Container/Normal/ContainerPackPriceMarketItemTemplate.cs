using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1489474099632)]
    public class ContainerPackPriceMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return ContainerUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
