﻿using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1454667308567L)]
	public class NotificationShownEvent : ECSEvent
	{
		public void Execute(Player player, Entity notification)
		{
			player.UnshareEntity(notification);
		}
	}
}
