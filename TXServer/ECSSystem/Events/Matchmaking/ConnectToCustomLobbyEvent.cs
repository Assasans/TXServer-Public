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
				// TODO: enable "open lobby" feature in custom battle lobby and let users join to battle with specific battle code
				if (!battle.IsMatchMaking)
			    {
					var fetchedBattle = ServerConnection.BattlePool[ServerConnection.BattlePool.Count - 1];

					if ((fetchedBattle.RedTeamPlayers.Count + fetchedBattle.BlueTeamPlayers.Count) >= fetchedBattle.MaxPlayers)
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