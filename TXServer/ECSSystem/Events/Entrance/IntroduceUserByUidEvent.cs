using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1439375251389)]
	public class IntroduceUserByUidEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			//todo ??
			PlayerData data = player.Server.Database.FetchPlayerData(Uid);
			data.Player = player;
			if (data != null)
			{
				player.Data = data;
				CommandManager.SendCommands(player, new SendEventCommand(new PersonalPasscodeEvent(), entity));
			}
		}

		public string Uid { get; set; }
	}
}
