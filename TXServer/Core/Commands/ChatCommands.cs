using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.Battles;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Events;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.Bonus;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Commands
{
    public static class ChatCommands
	{
		private static readonly Dictionary<string, (string, ChatCommandConditions, Func<Player, string[], string>)> Commands = new()
		{
			{ "help", ("help [opt: hackBattle]", ChatCommandConditions.None, Help) },
			{ "ping", (null, ChatCommandConditions.None, Ping) },

			{ "open", (null, ChatCommandConditions.Admin | ChatCommandConditions.InBattle, Open) },
			{ "start", (null, ChatCommandConditions.Admin | ChatCommandConditions.InBattle, Start) },
			{ "pause", (null, ChatCommandConditions.Admin | ChatCommandConditions.InBattle, Pause) },
			{ "finish", (null, ChatCommandConditions.Admin | ChatCommandConditions.InBattle, Finish) },
			{ "positionInfo", (null, ChatCommandConditions.Admin | ChatCommandConditions.InMatch, PositionInfo) },

			{ "stats", (null, ChatCommandConditions.Tester, Stats) },
			{ "spawnInfo", (null, ChatCommandConditions.Tester | ChatCommandConditions.InMatch, SpawnInfo) },

			{ "hackBattle", (null, ChatCommandConditions.BattleOwner, HackBattle) },
			{ "cheat", ("cheat [supply] [target]", ChatCommandConditions.HackBattle | ChatCommandConditions.ActiveTank, Cheat) },
			{ "flag", ("flag [color] [deliver/drop/return/give] [user]", ChatCommandConditions.HackBattle | ChatCommandConditions.Admin, FlagAction) },
			{ "goldrain", ("goldrain [number]", ChatCommandConditions.HackBattle | ChatCommandConditions.Admin, GoldboxRain) },
			{ "supplyrain", ("supplyrain", ChatCommandConditions.HackBattle | ChatCommandConditions.Admin, SupplyRain) },
			{ "gravity", ("gravity [number]", ChatCommandConditions.HackBattle | ChatCommandConditions.BattleOwner, Gravity) },
			{ "turretRotation", ("turretRotation [number]", ChatCommandConditions.HackBattle, TurretRotation) }
		};

		private static readonly Dictionary<ChatCommandConditions, string> ConditionErrors = new()
		{
			{ ChatCommandConditions.Admin, "You are not an admin" },
			{ ChatCommandConditions.Tester, "You are not a tester" },
			{ ChatCommandConditions.InBattle, "You are not in battle" },
			{ ChatCommandConditions.BattleOwner, "You do not own this battle" },
			{ ChatCommandConditions.HackBattle, "HackBattle is not enabled in this battle" },
			{ ChatCommandConditions.InMatch, "You are not in match" },
			{ ChatCommandConditions.ActiveTank, "Your tank is not active" },
		};

		/// <summary>
		/// Preprocesses command conditions
		/// </summary>
		static ChatCommands()
        {
			foreach (var _command in Commands)
			{
				var command = _command.Value;

				if (command.Item2.HasFlag(ChatCommandConditions.ActiveTank))
					command.Item2 |= ChatCommandConditions.InMatch;
				if ((command.Item2 & (ChatCommandConditions.BattleOwner | ChatCommandConditions.HackBattle | ChatCommandConditions.InMatch)) != 0)
                    command.Item2 |= ChatCommandConditions.InBattle;

				Commands[_command.Key] = command;
			}
        }

        public static string CheckForCommand(Player player, string message)
		{
			if (!message.StartsWith('/')) return null;
			string[] args = message[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (args.Length == 0) return null;

			if (Commands.TryGetValue(args[0], out var desc))
			{
				var playerConditions = GetConditionsFor(player);

				foreach (var condition in Enum.GetValues<ChatCommandConditions>())
                {
					if ((desc.Item2 & condition) == condition && (playerConditions & condition) != condition)
						return ConditionErrors[condition];
                }

				return desc.Item3(player, args[1..]);
			}

			return "Unknown command. Enter \"/help\" to view available commands";
		}

		private static string Help(Player player, string[] args)
        {
			ChatCommandConditions playerConditions = GetConditionsFor(player);
			string message;

			if (!args.Any())
			{
				message = '/' + string.Join("\n/", from command in Commands
				    where playerConditions.HasFlag(command.Value.Item2) &&
				          !command.Value.Item2.HasFlag(ChatCommandConditions.HackBattle)
				    select command.Value.Item1 ?? command.Key);
			}
			else
			{
				ChatCommandConditions condition;
				switch (args[0])
				{
					case "admin":
						condition = ChatCommandConditions.Admin;
						if (!player.Data.Admin)
							return ConditionErrors[condition];
						break;
					case "hackBattle":
						condition = ChatCommandConditions.HackBattle;
						if (!playerConditions.HasFlag(ChatCommandConditions.HackBattle))
							return ConditionErrors[condition];
						break;
					case "test":
						condition = ChatCommandConditions.Tester;
						if (!player.Data.Beta)
							return ConditionErrors[condition];
						break;
					default:
						return "Invalid command, specific help message wasn't found";
				}
				
				message = '/' + string.Join("\n/", from command in Commands
					where playerConditions.HasFlag(command.Value.Item2) &&
					      command.Value.Item2.HasFlag(condition)
					select command.Value.Item1 ?? command.Key);
			}
			return message;
        }

        private static string Ping(Player player, string[] args) => $"Network latency: {player.Connection.Ping} ms";

		private static string PositionInfo(Player player, string[] args)
        {
			Vector3 tankPosition = player.BattlePlayer.MatchPlayer.TankPosition;
			return $"X: {tankPosition.X}, Y: {tankPosition.Y}, Z: {tankPosition.Z}";
		}

		private static string Start(Player player, string[] args)
		{
			if (!player.BattlePlayer.Battle.IsMatchMaking)
			{
				player.BattlePlayer.Battle.BattleState = BattleState.Starting;
			}
			player.BattlePlayer.Battle.ForcePause = false;
			player.BattlePlayer.Battle.ForceStart = true;
			return "Started";
		}

		private static string Pause(Player player, string[] args)
		{
			Battle battle = player.BattlePlayer.Battle;
			
			if (!battle.ForceOpen && battle.ForcePause)
				return "Already paused";

			player.BattlePlayer.Battle.ForceStart = false;
			player.BattlePlayer.Battle.ForcePause = true;
			return "Paused battle entry";
		}
		
		private static string Finish(Player player, string[] args)
		{
			player.BattlePlayer.Battle.FinishBattle();
			return "Finished";
		}

		private static string FlagAction(Player player, string[] args)
		{
			if (player.BattlePlayer?.Battle.Params.BattleMode is not BattleMode.CTF) 
				return "Flag command is only available in CTF";
			if (args.Length < 2)
				return "Parsing error, '/flag' needs at least 2 arguments";
			if (args[0].ToUpper() is not "BLUE" and not "RED")
				return $"Parsing error, didn't find flag with the color '{args[0]}'";
			
			Battle battle = player.BattlePlayer.Battle;
			var modeHandler = (CTFHandler)battle.ModeHandler;
			BattleView teamView = modeHandler.BattleViewFor(player.BattlePlayer);
			Flag flag = args[0].ToUpper() == "BLUE" ? teamView.AllyTeamFlag : teamView.EnemyTeamFlag;
			Entity enemyTeamOfFlag = args[0].ToUpper() == "BLUE" ? teamView.EnemyTeamEntity : teamView.AllyTeamEntity;

			switch (args[1])
			{
				case "deliver":
					switch (flag.State)
					{
						case FlagState.Captured:
							string carrierUid = flag.Carrier.Player.Data.Username;
							flag.Deliver();
							battle.UpdateScore(enemyTeamOfFlag);
							return $"{carrierUid} delivered the {args[0].ToLower()} flag";
						case FlagState.Dropped:
							flag.Return(silent:true);
							break;
					}
					
					battle.MatchPlayers.Select(x => x.Player).SendEvent(new FlagDeliveryEvent(), flag.FlagEntity);
					battle.UpdateScore(enemyTeamOfFlag);
					return $"Delivered the {args[0].ToLower()} flag";
				case "drop":
					if (flag.State != FlagState.Captured)
						return "Flag state error, flag needs to be captured";
					
					flag.Drop(false);
					return $"Dropped the {args[0].ToLower()} flag";
				case "return":
					switch (flag.State)
					{
						case FlagState.Captured:
							flag.Drop(false);
							flag.Return();
							break;
						case FlagState.Dropped:
							flag.Return();
							break;
						case FlagState.Home:
							return "Flag state error, flag needs to be captured or dropped";
					}
					return $"Returned the {args[0].ToLower()} flag to its base";
				case "give":
					string targetName = "";
					targetName = args.Length < 3 ? player.Data.Username : args[2];
					Player target = battle.FindPlayerByUid(targetName);
					if (target == null)
						return $"Error, the user '{targetName}' wasn't found in this battle";
					if (target.BattlePlayer.Team == flag.Team)
						return $"Logical error, {targetName} can't capture the flag of this team";
					if (flag.Carrier == player.BattlePlayer)
						return $"Logical error, {targetName} already has the {args[0].ToLower()} flag";

					switch (flag.State)
					{
						case FlagState.Captured:
							flag.Drop(false);
							flag.Pickup(target.BattlePlayer);
							break;
						case FlagState.Home:
							flag.Capture(target.BattlePlayer);
							break;
						case FlagState.Dropped:
							flag.Pickup(target.BattlePlayer);
							break;
					}
					return $"Gave flag to user '{targetName}'";
				default:
					return $"Parsing error, '/flag' doesn't have an argument named '{args[1]}'";
			}
		}

		private static string Open(Player player, string[] args)
		{
			if (player.BattlePlayer.Battle.ForceOpen)
				return "Already opened";
			player.BattlePlayer.Battle.ForceOpen = true;
			return "Opened";
		}

		private static string SpawnInfo(Player player, string[] args)
		{
			SpawnPoint lastSpawnPoint = player.BattlePlayer.MatchPlayer.LastSpawnPoint;
			return $"{player.BattlePlayer.Battle.CurrentMapInfo.Name}, {player.BattlePlayer.Battle.Params.BattleMode}, " +
			       $"{player.User.GetComponent<TeamColorComponent>().TeamColor}, Number: {lastSpawnPoint.Number}";
		}

		private static string Stats(Player player, string[] args)
		{
			return $"Online players: {Server.Instance.Connection.Pool.Count(x => x.IsActive)}\n" +
				$"MM battles: {ServerConnection.BattlePool.Count(b => b.IsMatchMaking)}\n" +
				$"Custom battles: {ServerConnection.BattlePool.Count(b => !b.IsMatchMaking)}\n";
		}

		private static string HackBattle(Player player, string[] args)
		{
			if (player.BattlePlayer.Battle.IsMatchMaking)
				return "HackBattle cannot be enabled in matchmaking battles";

			if (((CustomBattleHandler)player.BattlePlayer.Battle.TypeHandler).HackBattle)
				return "HackBattle is already enabled";

			Dictionary<List<BattlePlayer>, Entity> targets = new()
			{
				{ player.BattlePlayer.Battle.AllBattlePlayers.ToList(), player.BattlePlayer.Battle.BattleLobbyChatEntity }
			};
			if (player.BattlePlayer.Battle.MatchPlayers.Any())
				targets.Add(player.BattlePlayer.Battle.MatchPlayers, player.BattlePlayer.Battle.GeneralBattleChatEntity);
			string notification = "This is now a \"HackBattle\"! The owner can cheat & change a lot of things. Note: this is very experimental.";
			MessageOtherPlayers(notification, player, targets);

			((CustomBattleHandler)player.BattlePlayer.Battle.TypeHandler).HackBattle = true;
			return "HackBattle was enabled";
		}

		private static string Gravity(Player player, string[] args)
		{
			Battle battle = player.BattlePlayer.Battle;

			string notification = "Gravity changed ";
			float gravity;
			if (args.Length == 0)
			{
				gravity = battle.GravityTypes[battle.Params.Gravity];
				notification += "to params gravity";
			}
			else
			{
				bool successfullyParsed = float.TryParse(args[0], out gravity);
				if (!successfullyParsed)
					return "Parsing error, '/gravity' only allows numbers or nothing as argument";
				notification += $"to {gravity}";
			}

			foreach (BattlePlayer battlePlayer in battle.MatchPlayers)
            {
				battle.KeepRunning = true;
				battlePlayer.Rejoin = true;
				battlePlayer.Player.SendEvent(new KickFromBattleEvent(), battlePlayer.MatchPlayer.BattleUser);
			}

			battle.BattleEntity.ChangeComponent<GravityComponent>(component => component.Gravity = gravity);
			
			Dictionary<List<BattlePlayer>, Entity> targets = new()
			{
				{ battle.AllBattlePlayers.ToList(), battle.BattleLobbyChatEntity }
			};

			MessageOtherPlayers(notification, player, targets);

			return notification;
		}

		private static string GoldboxRain(Player player, string[] args)
		{
			Battle battle = player.BattlePlayer.Battle;
			IEnumerable<BattleBonus> unusedGolds = battle.GoldBonuses
				.Where(b => b.State == BonusState.Unused).ToArray();

			if (!unusedGolds.Any())
				return "Weather error, it's not cloudy enough";
			if (args.Length != 0)
			{
				bool successfullyParsed = int.TryParse(args[0], out int amount);
				if (!successfullyParsed)
					return "Parsing error, '/goldrain' only allows integer or nothing as argument";

				if (unusedGolds.Count() < amount)
					return $"Weather error, not enough goldboxes available: {unusedGolds.Count()}";
				Random random = new();
				unusedGolds = unusedGolds.OrderBy(x => random.Next()).Take(amount);
			}

			foreach (BattleBonus goldBonus in unusedGolds)
			{
				goldBonus.CurrentCrystals = 0;
				goldBonus.State = BonusState.New;
				battle.MatchPlayers.Select(x => x.Player).SendEvent(new GoldScheduleNotificationEvent(""), 
					battle.RoundEntity);
			}
			
			return "It'll start raining gold soon, get an umbrella!";
		}
		
		private static string SupplyRain(Player player, string[] args)
		{
			Battle battle = player.BattlePlayer.Battle;
			if (player.BattlePlayer.Battle.Params.DisabledModules)
				return "Logical error, '/supplyrain' is only available when supplies are activated";

			foreach (BattleBonus battleBonus in battle.BattleBonuses)
				battleBonus.StateChangeCountdown = 20;

			return "It'll start raining soon, get an umbrella!";
		}

		private static string Cheat(Player player, string[] args)
		{
			switch (args[0].ToUpper())
			{
				case "ARMOR":
				case "DAMAGE":
				case "REPAIR":
				case "SPEED":
					string cheatTargets = "";
					if (args.Length > 1) cheatTargets = args[1];

					bool cheatActivated = EnableSupplyCheat(args[0], cheatTargets, player.BattlePlayer.MatchPlayer);
					if (cheatActivated)
						return "Cheat activated";
					else
						goto default;
				case "disable":
					int cheats = DisableSupplyCheats(player.BattlePlayer.MatchPlayer);
					return $"{cheats} cheat(s) disabled";
				default:
					return "Cheat or target not found";
			}
		}

		private static string TurretRotation(Player player, string[] args)
		{
			string notification = "Changed turret rotation ";
			float? rotationSpeed;

			if (args.Length == 0)
			{
				rotationSpeed = null;
				notification += "to normal";
			}
			else
			{
				bool successfullyParsed = float.TryParse(args[0], out float temp);
				
				if (!successfullyParsed)
					return "Parsing error, '/turretRotation' only allows numbers or nothing as argument";
				if (temp > 1000 || temp < 0)
					return "Out of range, '/turretRotation' only allows range from 0 to 1000";
				
				rotationSpeed = temp;
				notification += $"to {rotationSpeed}";
			}
			
			player.BattlePlayer.Battle.KeepRunning = true;
			player.BattlePlayer.Rejoin = true;
			player.BattlePlayer.Player.SendEvent(new KickFromBattleEvent(), player.BattlePlayer.MatchPlayer.BattleUser);

			player.BattlePlayer.TurretRotationSpeed = rotationSpeed;
			return notification;
		}

		private static ChatCommandConditions GetConditionsFor(Player player)
        {
			ChatCommandConditions conditions = 0;

			if (player.Data.Admin)
				conditions |= ChatCommandConditions.Admin;
			if (player.Data.Beta || player.Data.Admin)
				conditions |= ChatCommandConditions.Tester;

			if (player.IsInBattle)
			{
				conditions |= ChatCommandConditions.InBattle;

				if (player.IsBattleOwner || player.Data.Admin)
					conditions |= ChatCommandConditions.BattleOwner;
				if (player.BattlePlayer.Battle.TypeHandler is CustomBattleHandler {HackBattle: true} || player.Data.Admin)
					conditions |= ChatCommandConditions.HackBattle;

				if (player.IsInMatch)
					conditions |= ChatCommandConditions.InMatch;
				if (player.BattlePlayer.MatchPlayer?.TankState == TankState.Active)
					conditions |= ChatCommandConditions.ActiveTank;
			}

			return conditions;
		}

        private static bool EnableSupplyCheat(string type, string target, MatchPlayer matchPlayer)
		{
			bool supplyCheat = Enum.TryParse(type, out BonusType bonusType);

			if (!supplyCheat) return false;
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
					BattlePlayer specificTarget = matchPlayer.Battle.MatchPlayers.SingleOrDefault(player => player.User.GetComponent<UserUidComponent>().Uid == target);
					if (specificTarget != null)
					{
						_ = new SupplyEffect(bonusType, specificTarget.MatchPlayer, cheat: true);
						return true;
					}
					break;
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
	}
}