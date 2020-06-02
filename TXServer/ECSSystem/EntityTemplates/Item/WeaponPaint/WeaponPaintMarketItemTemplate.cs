using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636287153836461132L)]
    public class WeaponPaintMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return WeaponPaintUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
