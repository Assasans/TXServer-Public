﻿using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Entrance;

namespace TXServer.ECSSystem.Events.Entrance.Invite
{
    [SerialVersionUID(1439810001590L)]
	public class InviteEnteredEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			// Todo: database immigration
			List<string> AllowedUids = new()
			{
				"NoNick", "Tim203", "M8", "Kaveman", "Assasans",
				"Concodroid", "Corpserdefg",
				"SH42913",
				"Bodr", "C6OI", "Legendar-X", "Pchelik", "networkspecter", "DageLV",
				"Black_Wolf", "NN77296", "MEWPASCO", "Doctor", "TowerCrusher", "Kurays", "AlveroHUN, Inctrice"
			};

			string inviteCode = entity.GetComponent<InviteComponent>().InviteCode;
			if (AllowedUids.Contains(inviteCode) || AllowedUids.Contains(inviteCode.ToLower()))
			{
				player.SendEvent(new CommenceRegistrationEvent(), entity);
				Logger.Log($"{player}: New session with invite code \"{inviteCode}\"");
			}
			else
			{
				player.SendEvent(new InviteDoesNotExistEvent(), entity);
				Logger.Log($"{player}: Invalid invite code \"{inviteCode}\"");
			}
		}
	}
}
