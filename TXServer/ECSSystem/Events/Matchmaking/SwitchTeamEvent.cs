using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.ECSSystem.Components;
using System.Linq;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1499172594697L)]
	public class SwitchTeamEvent : ECSEvent
	{
		public void Execute(Player player, Entity mode)
		{
			Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.AllBattlePlayers.Contains(player.BattleLobbyPlayer));
			TeamColor newTeamColor;

			if (battle.BattleParams.BattleMode == BattleMode.DM)
				return;

			if (battle.RedTeamPlayers.Contains(player.BattleLobbyPlayer))
			{
				newTeamColor = TeamColor.BLUE;
				battle.RedTeamPlayers.Remove(player.BattleLobbyPlayer);
				player.BattleLobbyPlayer = new BattleLobbyPlayer(player, battle.BlueTeamEntity);
				battle.BlueTeamPlayers.Add(player.BattleLobbyPlayer);
				UserResult userResult = battle.RedTeamResults.Single(r => r.UserId == player.User.EntityId);
				battle.RedTeamResults.Remove(userResult);
				battle.BlueTeamResults.Add(userResult);
			}
			else
            {
				newTeamColor = TeamColor.RED;
				battle.BlueTeamPlayers.Remove(player.BattleLobbyPlayer);
				player.BattleLobbyPlayer = new BattleLobbyPlayer(player, battle.RedTeamEntity);
				battle.RedTeamPlayers.Add(player.BattleLobbyPlayer);
				UserResult userResult = battle.BlueTeamResults.Single(r => r.UserId == player.User.EntityId);
				battle.BlueTeamResults.Remove(userResult);
				battle.RedTeamResults.Add(userResult);
			}
			player.User.RemoveComponent<TeamColorComponent>();
			player.User.AddComponent(new TeamColorComponent(newTeamColor));
		}
	}
}