using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1453881244963L)]
	public class IncrementRegistrationNicksEvent : ECSEvent
	{
		//public void Execute(Player player, Entity clientSession)
		//{
		// TODO: do something with this
		//}

		public string Nick { get; set; }
	}
}
