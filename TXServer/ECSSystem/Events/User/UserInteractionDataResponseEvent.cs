using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1412360987645)]
	public class UserInteractionDataResponseEvent : ECSEvent
	{
		public UserInteractionDataResponseEvent(long userId, string userUid, bool canRequestFriendship, bool friendshipRequestWasSend, bool muted, bool reported)
		{
			UserId = userId;
			UserUid = userUid;
			CanRequestFrendship = canRequestFriendship;
			FriendshipRequestWasSend = friendshipRequestWasSend;
			Muted = muted;
			Reported = reported;
		}
		
		public long UserId { get; set; }
		public string UserUid { get; set; }
		public bool CanRequestFrendship { get; set; }
		public bool FriendshipRequestWasSend { get; set; }
		public bool Muted { get; set; }
		public bool Reported { get; set; }
	}
}
