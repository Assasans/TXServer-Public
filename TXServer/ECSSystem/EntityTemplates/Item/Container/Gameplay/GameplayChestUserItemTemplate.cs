using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1486562494879L)]
    public class GameplayChestUserItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user) => ContainerUserItemTemplate.CreateEntity(new GameplayChestMarketItemTemplate(), marketItem, user);
    }
}
