using System;

using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1454623211245)]
	public class UserInteractionDataRequestEvent : ECSEvent
	{
		public long UserId { get; set; }

		public void Execute(Player player, Entity entity)
		{
			Player targetPlayer = player.Server.FindPlayerById(UserId);
			player.SendEvent(new UserInteractionDataResponseEvent(
				targetPlayer.User.EntityId,
				targetPlayer.Data.UniqueId,
				true,
				false,
				false,
				false
			), entity);
		}
	}
}
