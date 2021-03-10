using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Entrance
{
	[SerialVersionUID(1438590245672L)]
	public class RequestRegisterUserEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{ 
		}
		public string Uid { get; set; }
		public string EncryptedPasswordDigest { get; set; }
		public string Email { get; set; }
		public string HardwareFingerprint { get; set; }
		public bool Subscribed { get; set; }
		public bool Steam { get; set; }
		public bool QuickRegistration { get; set; }
	}
}
