using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
	public class ElevatedAccessUserBasePunishEvent : ECSEvent
	{
		public string Uid { get; set; }
		public string Reason { get; set; }
	}
}