using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1458846544326)]
	public class IntroduceUserByEmailEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			PlayerData data = player.Server.Database.FetchPlayerDataByEmail(Email);
			if (data == null) return; // Player#LogIn(Entity) will kick the player
			data.Player = player;
			player.Data = data;
			CommandManager.SendCommands(player, new SendEventCommand(new PersonalPasscodeEvent(), entity));
		}

		public string Email { get; set; }
	}
}
