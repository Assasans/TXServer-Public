using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.Core;
using System;
using TXServer.Core.Commands;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1547616531111L)]
	public class ConnectToCustomLobbyEvent : ECSEvent
	{
		public void Execute(Player player, Entity mode)
		{
			

			bool found = false;
			foreach (TXServer.Core.Battles.Battle battle in ServerConnection.BattlePool.ToArray())
			{
				// TODO: let users join with battle entity as battle code
				if (battle.IsOpen)
			    {
					var fetchedBattle = ServerConnection.BattlePool[ServerConnection.BattlePool.Count - 1];

					if ((fetchedBattle.RedTeamPlayers.Count + fetchedBattle.BlueTeamPlayers.Count) >= fetchedBattle.BattleParams.MaxPlayers)
                    {
						CommandManager.SendCommands(player,
						new SendEventCommand(new EnterBattleLobbyFailedEvent(alreadyInLobby:false, lobbyIsFull:true), player.User));
					}
					else
                    {
						ServerConnection.BattlePool[ServerConnection.BattlePool.Count - 1].AddPlayer(player);
					}
			        found = true;
			        break;
			    }
			}
			if (!found)
			{
				CommandManager.SendCommands(player,
						new SendEventCommand(new CustomLobbyNotExistsEvent(), player.User));
			}
		}
		public long LobbyId { get; set; }
	}
}