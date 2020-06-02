using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1487149202122)]
    public class GameplayChestMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return GameplayChestUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
