using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(636469080057216111L)]
	public class CreatePrivateChatEvent : ECSEvent
	{
		// TODO Execute

		public string UserUid { get; set; }
	}
}
