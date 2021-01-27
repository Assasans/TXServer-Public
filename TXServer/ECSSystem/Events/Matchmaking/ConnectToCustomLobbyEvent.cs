using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.Core;
using TXServer.Core.Commands;
using System.Linq;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1547616531111L)]
	public class ConnectToCustomLobbyEvent : ECSEvent
	{
		public void Execute(Player player, Entity mode)
		{
			// admins can enter with the "last" code the newest custom lobby
			Core.Battles.Battle battle;
			if (LobbyId == 1211920 && player.User.GetComponent<UserAdminComponent>() != null)
			{
				battle = ServerConnection.BattlePool.LastOrDefault(b => !b.IsMatchMaking && b.IsOpen);
			} else {
				battle = ServerConnection.BattlePool.FirstOrDefault(b => b.BattleLobbyEntity.GetComponent<BattleLobbyGroupComponent>().Key == LobbyId && b.IsOpen);
			}

			if (battle != null)
			{
				if ((battle.RedTeamPlayers.Count + battle.BlueTeamPlayers.Count) >= battle.BattleParams.MaxPlayers)
				{
					CommandManager.SendCommands(player,
						new SendEventCommand(new EnterBattleLobbyFailedEvent(alreadyInLobby: false, lobbyIsFull: true), player.User));
				} else {
					battle.AddPlayer(player);
				}
			} else {
				CommandManager.SendCommands(player,
				new SendEventCommand(new CustomLobbyNotExistsEvent(), player.User));
			}
		}
		public long LobbyId { get; set; }
	}
}