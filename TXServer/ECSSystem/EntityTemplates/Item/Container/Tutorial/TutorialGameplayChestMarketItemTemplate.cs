﻿using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636413290399070700L)]
	public class TutorialGameplayChestMarketItemTemplate : IMarketItemTemplate
	{
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            return TutorialGameplayChestUserItemTemplate.CreateEntity(marketItem, user);
        }
    }
}
