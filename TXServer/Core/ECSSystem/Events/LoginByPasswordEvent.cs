using TXServer.Bits;
using TXServer.Core.Commands;

namespace TXServer.Core.ECSSystem.Events
{
    [SerialVersionUID(1437480091995)]
	public class LoginByPasswordEvent : ECSEvent
	{
		[Protocol] public string HardwareFingerprint { get; set; } = "";

		[Protocol] public string PasswordEncipher { get; set; } = "";

		[Protocol] public bool RememberMe { get; set; }

		public override void Execute(Entity entity)
		{
			CommandManager.SendCommands(Player.Instance.Value.Socket,
				new SendEventCommand(new LoginFailedEvent(), entity),
				new SendEventCommand(new InvalidPasswordEvent(), entity));
		}
	}
}
