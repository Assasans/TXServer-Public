﻿using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1458846544326)]
	public class IntroduceUserByEmailEvent : ECSEvent
	{
		public override void Execute(Entity entity)
		{
			Player.Instance.Uid = "guest";
			Player.Instance.Email = Email;
			CommandManager.SendCommands(Player.Instance.Socket, new SendEventCommand(new PersonalPasscodeEvent(), entity));
		}

		public string Email { get; set; }
	}
}
