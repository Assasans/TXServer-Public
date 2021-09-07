using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1458846544326)]
	public class IntroduceUserByEmailEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
            if (!player.CheckInviteCode(Email))
            {
                player.SendEvent(new UidInvalidEvent(), entity);
                player.SendEvent(new LoginFailedEvent(), entity);
                return;
            }

			PlayerData data = player.Server.Database.GetPlayerDataByEmail(Email);
			if (data == null) return; // Player#LogIn(Entity) will kick the player
			data.Player = player;
			player.Data = data;
			player.SendEvent(new PersonalPasscodeEvent(), entity);
		}

		public string Email { get; set; }
	}
}
