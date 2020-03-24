﻿using TXServer.Core.Commands;
using TXServer.Core.ECSSystem.Components;
using TXServer.Core.ECSSystem.EntityTemplates;
using static TXServer.Core.ECSSystem.Entity;

namespace TXServer.Core.ECSSystem.Events
{
	[SerialVersionUID(1437480091995)]
	public class LoginByPasswordEvent : ECSEvent
	{
		public override void Execute(Entity entity)
		{
			/*
			CommandManager.SendCommands(Player.Instance.Socket,
				new SendEventCommand(new LoginFailedEvent(), entity),
				new SendEventCommand(new InvalidPasswordEvent(), entity));
			*/

			CommandManager.SendCommands(Player.Instance.Socket,
				new ComponentAddCommand(entity, new UserGroupComponent()),
				new EntityShareCommand(GlobalEntities.FRACTIONSCOMPETITION),
				new EntityShareCommand(GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_FRONTIER),
				new EntityShareCommand(GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_ANTAEUS));
				//new SendEventCommand());
		}

		[Protocol] public string HardwareFingerprint { get; set; } = "";
		[Protocol] public string PasswordEncipher { get; set; } = "";
		[Protocol] public bool RememberMe { get; set; }
	}
}
