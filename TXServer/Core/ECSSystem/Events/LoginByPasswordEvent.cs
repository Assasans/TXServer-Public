using System.Collections.Generic;
using TXServer.Core.Commands;
using TXServer.Core.ECSSystem.Components;

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
			/*
			CommandManager.SendCommands(Player.Instance.Socket,
				new SendEventCommand(new LoginFailedEvent(), entity),
				new SendEventCommand(new InvalidPasswordEvent(), entity));
			*/

			Entity testEntity = new Entity(new TemplateAccessor(new EntityTemplates.FractionsCompetitionTemplate(), "fractionscompetition"), new List<Component>());

			CommandManager.SendCommands(Player.Instance.Socket,
				new ComponentAddCommand(entity, new UserGroupComponent()),
				new EntityShareCommand(testEntity));
		}
	}
}
