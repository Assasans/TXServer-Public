using Serilog;
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
        private static readonly ILogger Logger = Log.Logger.ForType<InviteEnteredEvent>();

		public void Execute(Player player, Entity entity)
		{
            string inviteCode = entity.GetComponent<InviteComponent>().InviteCode;
			if (player.Server.Database.TryGetInvite(inviteCode, out var invite))
            {
                player.Invite = invite;
				player.SendEvent(new CommenceRegistrationEvent(), entity);

				Logger.WithPlayer(player).Information(
                    "New session with invite code {Code}",
                    inviteCode
                );
			}
			else
			{
				player.SendEvent(new InviteDoesNotExistEvent(), entity);
                Logger.WithPlayer(player).Information("Invalid invite code {Code}", inviteCode);
			}
		}
	}
}
