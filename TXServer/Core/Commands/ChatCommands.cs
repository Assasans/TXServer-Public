using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.Battles;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Events;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Commands
{
	public static class ChatCommands
	{
		public static string CheckForCommand(Player player, string message)
		{
			if (!message.StartsWith('/')) return null;
			string[] args = message.TrimStart('/').Split(' ', StringSplitOptions.RemoveEmptyEntries);

			bool hackBattle = player.IsOwner() && (player.BattlePlayer.Battle.TypeHandler as CustomBattleHandler).HackBattle;

			if (args.Length == 0) return null;

			if (player.IsOwner() || player.Data.Admin)
            {
				if (args[0] == "hackBattle")
                {
					if ((player.BattlePlayer.Battle.TypeHandler as CustomBattleHandler).HackBattle)
						return "HackBattle is already enabled";

					Dictionary<List<BattlePlayer>, Entity> targets = new() { 
						{ player.BattlePlayer.Battle.AllBattlePlayers.ToList(), player.BattlePlayer.Battle.BattleLobbyChatEntity } };
					if (player.BattlePlayer.Battle.MatchPlayers.Any()) 
						targets.Add(player.BattlePlayer.Battle.MatchPlayers, player.BattlePlayer.Battle.GeneralBattleChatEntity);
					string notification = "This is now a 'HackBattle'! The owner can cheat & change a lot of things. Note: this is very experimental.";
					MessageOtherPlayers(notification, player, targets);

					(player.BattlePlayer.Battle.TypeHandler as CustomBattleHandler).HackBattle = true;
					return "HackBattle was enabled";
                }

				if (hackBattle || player.Data.Admin)
                {
					switch (args[0])
					{
						case "cheat":
							if (!player.IsInMatch()) return null;
							switch (args[1])
							{
								case "ARMOR":
								case "DAMAGE":
								case "REPAIR":
								case "SPEED":
									string cheatTargets = "";
									if (args.Length > 2) cheatTargets = args[2];
									bool cheatActivated = EnableSupplyCheat(args[1], cheatTargets, player.BattlePlayer.MatchPlayer);
									if (cheatActivated) return "Cheat activated";
									else return "Cheat or target not found";
								case "disable":
									int cheats = DisableSupplyCheats(player.BattlePlayer.MatchPlayer);
									return $"{cheats} cheat(s) disabled";
								default:
									return null;
							}
						case "gravity":
							if (!player.IsInBattleLobby()) return null;

							string notification = "Gravity changed ";
							float gravity;
							if (args.Length < 2)
							{
								gravity = player.BattlePlayer.Battle.GravityTypes[player.BattlePlayer.Battle.Params.Gravity];
								notification += "to params gravity";
							}
							else
							{
								bool successfullyParsed = float.TryParse(args[1], out gravity);
								if (!successfullyParsed)
									return "Parsing error, '/gravity' only allows numbers or nothing as argument";
								notification += $"to {gravity}";
							}
							
							player.BattlePlayer.Battle.BattleEntity.ChangeComponent<GravityComponent>(component => component.Gravity = gravity);
							
							if (player.BattlePlayer.Battle.MatchPlayers.Any()) notification += ": All match players have to rejoin or it will get glitchy";
							Dictionary<List<BattlePlayer>, Entity> targets = new() {
								{ player.BattlePlayer.Battle.AllBattlePlayers.ToList(), player.BattlePlayer.Battle.BattleLobbyChatEntity }};
							if (player.BattlePlayer.Battle.MatchPlayers.Any())
								targets.Add(player.BattlePlayer.Battle.MatchPlayers, player.BattlePlayer.Battle.GeneralBattleChatEntity);
							MessageOtherPlayers(notification, player, targets);

							return notification;
					}
				}
				
			}

			if (player.Data.Admin)
			{
				// admin only commands
				switch (args[0])
				{
					case "open":
						if (!player.IsInMatch()) return null;
						if (player.BattlePlayer.Battle.ForceOpen)
							return "Already opened";
						player.BattlePlayer.Battle.ForceOpen = true;
						return "Opened";
					case "positionInfo":
						if (!player.IsInMatch()) return null;
						Vector3 tankPosition = player.BattlePlayer.MatchPlayer.TankPosition;
						return ($"X: {tankPosition.X}, Y: {tankPosition.Y}, Z: {tankPosition.Z}");
					case "start":
						if (!player.IsInBattleLobby() || player.IsInMatch()) return null;
						if (!player.BattlePlayer.Battle.IsMatchMaking)
							player.BattlePlayer.Battle.BattleState = BattleState.Starting;
						else
							player.BattlePlayer.Battle.ForceStart = true;
						return "Started";
				}
			}

			if (player.Data.Beta || player.Data.Admin)
			{
				// tester & admin commands
				switch (args[0])
				{
					case "spawnInfo":
						if (!player.IsInMatch()) return null;
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

				if (player.Data.Beta)
					list = list.Concat(TesterCommands);
				if (player.Data.Admin)
					list = list.Concat(AdminCommands);

				return string.Join('\n', list);
			}

			return null;
		}


        private static bool EnableSupplyCheat(string type, string target, MatchPlayer matchPlayer)
		{
			bool supplieCheat = Enum.TryParse(type, out BonusType bonusType);

			if (supplieCheat)
			{
				switch (target)
				{
					case "":
						_ = new SupplyEffect(bonusType, matchPlayer, cheat: true);
						return true;
					case "all":
						foreach (BattlePlayer battlePlayer in matchPlayer.Battle.MatchPlayers)
							_ = new SupplyEffect(bonusType, battlePlayer.MatchPlayer, cheat: true);
						return true;
					default:
						BattlePlayer specificTarget = matchPlayer.Battle.MatchPlayers.Single(player => player.User.GetComponent<UserUidComponent>().Uid == target);
						if (specificTarget != null)
						{
							_ = new SupplyEffect(bonusType, specificTarget.MatchPlayer, cheat: true);
							return true;
						}
						break;
				}
			}
			return false;
		}

		private static int DisableSupplyCheats(MatchPlayer matchPlayer)
		{
			int cheats = 0;
			foreach (BattlePlayer battlePlayer in matchPlayer.Battle.MatchPlayers)
				foreach (SupplyEffect supplyEffect in battlePlayer.MatchPlayer.SupplyEffects.ToArray().Where(supply => supply.Cheat))
				{
					cheats += 1;
					supplyEffect.Remove();
				}
			return cheats;
		}

		private static void MessageOtherPlayers(string message, Player selfPlayer, Dictionary<List<BattlePlayer>, Entity> targets)
        {
			foreach (KeyValuePair<List<BattlePlayer>, Entity> target in targets)
			{
				target.Key.Where(battlePlayer => battlePlayer.Player != selfPlayer).Select(x => x.Player).SendEvent(new ChatMessageReceivedEvent
				{
					Message = message,
					SystemMessage = true,
					UserId = selfPlayer.User.EntityId,
					UserUid = selfPlayer.User.GetComponent<UserUidComponent>().Uid,
					UserAvatarId = selfPlayer.User.GetComponent<UserAvatarComponent>().Id
				}, target.Value);
			}
		}

		private static readonly List<string> TesterCommands = new() { "/spawnInfo", "/stats" };
		private static readonly List<string> AdminCommands = new() { "/positionInfo", "/start" };
		private static readonly List<string> HackBattleCommands = new() { "/cheat [SUPPLY] [target]", "/gravity [number]" };
	}
}