using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.Core;
using TXServer.Core.Commands;
using System.Linq;
using TXServer.ECSSystem.Components;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1547616531111L)]
	public class ConnectToCustomLobbyEvent : ECSEvent
	{
		public void Execute(Player player, Entity mode)
		{
			Core.Battles.Battle battle = ServerConnection.BattlePool.LastOrDefault(b => b.BattleLobbyEntity.EntityId == LobbyId);

			// admins can enter with the "last" code the newest custom lobby
			if (player.User.GetComponent<UserAdminComponent>() != null) {
				switch (LobbyId)
				{
					// soft join latest open lobby
					case 1211920:
						battle = ServerConnection.BattlePool.LastOrDefault(b => ((b.TypeHandler as CustomBattleHandler)?.IsOpen).GetValueOrDefault());
						break;
					// brute join latest lobby
					case 21211920:
						battle = ServerConnection.BattlePool.LastOrDefault(b => !b.IsMatchMaking);
						break;
				}
			}

			if (battle != null)
			{
				if (battle.AllBattlePlayers.Count() >= battle.Params.MaxPlayers)
				{
					player.SendEvent(new EnterBattleLobbyFailedEvent(alreadyInLobby: false, lobbyIsFull: true), player.User);
				} else {
					battle.AddPlayer(player);
				}
			} 
			else
			{
				player.SendEvent(new CustomLobbyNotExistsEvent(), player.User);
			}
		}
		public long LobbyId { get; set; }
	}
}