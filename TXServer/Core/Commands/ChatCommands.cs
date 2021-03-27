using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.Battles;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Events.Battle;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Commands
{
	public class ChatCommands
	{
		public static string CheckForCommand(Player player, string message)
		{
			bool testerPermission = player.User.GetComponent<ClosedBetaQuestAchievementComponent>() != null || player.User.GetComponent<UserAdminComponent>() != null;
			bool adminPermission = player.User.GetComponent<UserAdminComponent>() != null;
			bool playerInLobby = player.BattlePlayer != null;
			bool playerInBattle = playerInLobby && player.BattlePlayer.MatchPlayer != null;

			if (message == "/positionInfo" && playerInBattle && adminPermission)
			{
				Vector3 tankPosition = player.BattlePlayer.MatchPlayer.TankPosition;
				return ($"X: {tankPosition.X}, Y: {tankPosition.Y}, Z: {tankPosition.Z}");
			}

			if (message == "/spawnInfo" && playerInBattle && testerPermission)
			{
				SpawnPoint lastSpawnPoint = player.BattlePlayer.MatchPlayer.LastSpawnPoint;
				return ($"{player.BattlePlayer.Battle.CurrentMapInfo.Name}, {player.BattlePlayer.Battle.Params.BattleMode}, {player.User.GetComponent<TeamColorComponent>().TeamColor}, Number: {lastSpawnPoint.Number}");
			}

			if (message == "/start" && playerInLobby && adminPermission)
			{
				if (!player.BattlePlayer.Battle.IsMatchMaking)
					player.BattlePlayer.Battle.BattleState = BattleState.Starting;
				else
					player.BattlePlayer.Battle.ForceStart = true;
				return "Started";
			}

			if (message == "/stats" && testerPermission)
            {
				return $"MM battles: {ServerConnection.BattlePool.Where(b => b.IsMatchMaking).Count()}\n" +
					$"Custom battles: {ServerConnection.BattlePool.Where(b => !b.IsMatchMaking).Count()}";
			}

			if (message == "/help" && (testerPermission || adminPermission))
            {
				string reply = "";
				if (testerPermission) 
					foreach (string command in TesterCommands) reply += $"\n{command}";
				if (adminPermission)
					foreach (string command in AdminCommands) reply += $"\n{command}";
				return reply;
			}
			return null;
		}
		private static readonly List<string> TesterCommands = new() { "/spawnInfo", "/stats" };
		private static readonly List<string> AdminCommands = new() { "/positionInfo", "/start" };
	}
}