using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.Core;
using TXServer.ECSSystem.Components.Entrance;

namespace TXServer.ECSSystem.Events.Entrance.Invite
{
	[SerialVersionUID(1439810001590L)]
	public class InviteEnteredEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			// Todo: database immigration
			string inviteCode = entity.GetComponent<InviteComponent>().InviteCode;
			//player.SendEvent(new InviteDoesNotExistEvent(), entity);
			player.SendEvent(new CommenceRegistrationEvent(), entity);
		}
	}
}
