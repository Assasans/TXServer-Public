using System;
using System.Collections.Generic;
using System.Linq;
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
            string inviteCode = entity.GetComponent<InviteComponent>().InviteCode;
			if (player.Server.Database.IsInviteValid(inviteCode))
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
