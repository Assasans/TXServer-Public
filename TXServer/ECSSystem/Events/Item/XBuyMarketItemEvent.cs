﻿using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(1473424321578L)]
	public class XBuyMarketItemEvent : ECSEvent
	{
		public void Execute(Player player, Entity user, Entity item)
        {
            player.Data.XCrystals -= Price;
            BuyMarketItemEvent.HandleNewItem(player, item);
        }

		public int Price { get; set; }
        public int Amount { get; set; }
    }
}
