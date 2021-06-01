using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User
{
	[SerialVersionUID(1454623211245)]
	public class UserInteractionDataRequestEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			Player targetPlayer = Server.Instance.FindPlayerByUid(UserId);
			PlayerData data = player.Data;

			bool canRequestFriendship = !data.IncomingFriendIds.Concat(data.AcceptedFriendIds)
				.Concat(data.OutgoingFriendIds).Concat(data.BlockedPlayerIds).Concat(targetPlayer.Data.BlockedPlayerIds)
				.Contains(UserId);

			player.SendEvent(new UserInteractionDataResponseEvent(
				UserId,
				targetPlayer.Data.Username,
				canRequestFriendship,
				data.OutgoingFriendIds.Contains(UserId),
				data.BlockedPlayerIds.Contains(UserId),
				data.ReportedPlayerIds.Contains(UserId)
			), entity);
		}
		
		public long UserId { get; set; }
	}
	
}
