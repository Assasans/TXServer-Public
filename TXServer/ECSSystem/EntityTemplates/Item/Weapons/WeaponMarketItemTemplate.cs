using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    public abstract class WeaponMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return WeaponUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
