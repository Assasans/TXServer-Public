using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.Core;
using TXServer.ECSSystem.Components.Entrance;
using System.Collections.Generic;
using TXServer.Core.Logging;

namespace TXServer.ECSSystem.Events.Entrance.Invite
{
	[SerialVersionUID(1439810001590L)]
	public class InviteEnteredEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			// Todo: database immigration
			List<string> AllowedUids = new() { "NoNick", "Tim203", "M8", "Kaveman", "Concodroid", "Corpserdefg", "SH42913", "Bodr", "C6OI", "Legendar-X", "Pchelik", "networkspecter", "DageLV" };
			string inviteCode = entity.GetComponent<InviteComponent>().InviteCode;
			if (AllowedUids.Contains(inviteCode) || AllowedUids.Contains(inviteCode.ToLower()))
            {
				Logger.Log($"{entity.EntityId}: New session with invite code '{inviteCode}'");
				player.SendEvent(new CommenceRegistrationEvent(), entity);
			}
			else
			    player.SendEvent(new InviteDoesNotExistEvent(), entity);
		}
	}
}
