using System;
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
	public static class ChatCommands
	{
		public static string CheckForCommand(Player player, string message)
		{
			if (!message.StartsWith('/')) return null;
			string[] args = message.TrimStart('/').Split(' ', StringSplitOptions.RemoveEmptyEntries);

			bool isAdmin = player.User.HasComponent<UserAdminComponent>();
			bool isTester = isAdmin || player.User.HasComponent<ClosedBetaQuestAchievementComponent>();

			bool playerInBattle = player.BattlePlayer != null;
			bool playerInMatch = playerInBattle && player.BattlePlayer.MatchPlayer != null;

			if (isAdmin)
            {
				// admin only commands
				switch (args[0])
                {
					case "positionInfo":
						if (!playerInMatch) return null;
						Vector3 tankPosition = player.BattlePlayer.MatchPlayer.TankPosition;
						return ($"X: {tankPosition.X}, Y: {tankPosition.Y}, Z: {tankPosition.Z}");
					case "start":
						if (!playerInBattle || playerInMatch) return null;
						if (!player.BattlePlayer.Battle.IsMatchMaking)
							player.BattlePlayer.Battle.BattleState = BattleState.Starting;
						else
							player.BattlePlayer.Battle.ForceStart = true;
						return "Started";
				}
            }

			if (isTester)
			{
				// tester & admin commands
				switch (args[0])
				{
					case "spawnInfo":
						if (!playerInMatch) return null;
						SpawnPoint lastSpawnPoint = player.BattlePlayer.MatchPlayer.LastSpawnPoint;
						return ($"{player.BattlePlayer.Battle.CurrentMapInfo.Name}, {player.BattlePlayer.Battle.Params.BattleMode}, {player.User.GetComponent<TeamColorComponent>().TeamColor}, Number: {lastSpawnPoint.Number}");
					case "stats":
						return $"Online players: {Server.Instance.Connection.Pool.Count}\n" + 
							$"MM battles: {ServerConnection.BattlePool.Count(b => b.IsMatchMaking)}\n" +
							$"Custom battles: {ServerConnection.BattlePool.Count(b => !b.IsMatchMaking)}";
				}
			}

			if (args[0] == "help")
            {
				IEnumerable<string> list = Enumerable.Empty<string>();

				if (isTester)
					list = list.Concat(TesterCommands);
				if (isAdmin)
					list = list.Concat(AdminCommands);

				return string.Join('\n', list);
			}

			return null;
		}
		private static readonly List<string> TesterCommands = new() { "/spawnInfo", "/stats" };
		private static readonly List<string> AdminCommands = new() { "/positionInfo", "/start" };
	}
}