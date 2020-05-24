using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1450950140104L)]
	public class ChatMessageReceivedEvent : ECSEvent
	{
		public string Message { get; set; }

		public bool SystemMessage { get; set; }

		public string UserUid { get; set; }

		public long UserId { get; set; }

		public string UserAvatarId { get; set; }
	}
}
