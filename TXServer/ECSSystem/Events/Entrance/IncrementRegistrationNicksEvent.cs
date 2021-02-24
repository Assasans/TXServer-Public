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
		// Note from Kaveman: The database already assigns a unique ID to each player (int)... but then again I could be misunderstanding the event here
		//}

		public string Nick { get; set; }
	}
}
