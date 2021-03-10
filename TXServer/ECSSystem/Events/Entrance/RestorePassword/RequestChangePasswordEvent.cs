using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Entrance.RestorePassword
{
	[SerialVersionUID(1460403525230L)]
	public class RequestChangePasswordEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			// TODO: save in database + log in
		}
		public string PasswordDigest { get; set; }
		public string HardwareFingerprint { get; set; }
	}
}
