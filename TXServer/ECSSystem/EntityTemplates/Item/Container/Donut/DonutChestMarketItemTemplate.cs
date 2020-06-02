using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636408122917164205L)]
    public class DonutChestMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return SimpleChestUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
