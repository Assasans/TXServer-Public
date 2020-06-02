using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1469607574709L)]
    public class WeaponSkinMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return WeaponSkinUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
