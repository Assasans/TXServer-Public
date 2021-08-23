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
            // TODO(Assasans): Save in database & login
            // byte[] passwordHash = player.EncryptionComponent.RsaDecrypt(Convert.FromBase64String(PasswordDigest));

            // player.Data.PasswordHash = passwordHash;
            // player.Data.HardwareId = HardwareFingerprint;
            // player.Data.AutoLoginToken = null;
        }

		public string PasswordDigest { get; set; }
		public string HardwareFingerprint { get; set; }
	}
}
