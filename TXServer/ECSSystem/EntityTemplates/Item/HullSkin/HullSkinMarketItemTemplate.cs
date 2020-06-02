﻿using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1469607967377L)]
    public class HullSkinMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return HullSkinUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
