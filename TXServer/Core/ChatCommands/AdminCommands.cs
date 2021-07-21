using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Notification;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.Bonus;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.ChatCommands
{
    public static class AdminCommands
    {
        private static readonly Dictionary<string, (string, ChatCommandConditions, Func<Player, string[], string>)> Commands = new()
        {
            { "battlemode", ("battlemode [opt: shortcut]", ChatCommandConditions.InactiveBattle, ChangeBattleMode) },
            { "dailybonus", (null, ChatCommandConditions.None, DailyBonusRecharge)},
            { "finish", (null, ChatCommandConditions.ActiveBattle, Finish) },
            { "competition", ("competition [start/finish/reset]", ChatCommandConditions.None, FractionsCompetitionEditor )},
            { "friendlyfire", (null, ChatCommandConditions.InactiveBattle, ChangeFriendlyFire) },
            { "goldrain", (null, ChatCommandConditions.ActiveBattle, GoldboxRain) },
            { "immune", (null, ChatCommandConditions.InBattle, Immune) },
            { "map", ("map [opt: map name]", ChatCommandConditions.InactiveBattle, ChangeMap) },
            { "message", ("message [all/uid] [message]", ChatCommandConditions.None, Message) },
            { "modules", (null, ChatCommandConditions.InactiveBattle, ChangeModules) },
            { "open", (null, ChatCommandConditions.InBattle, Open) },
            { "pause", (null, ChatCommandConditions.InBattle, Pause) },
            { "positioninfo", (null, ChatCommandConditions.InMatch, PositionInfo) },
            { "recruitreward", ("recruitReward [opt: check/reset]", ChatCommandConditions.None, RecruitReward) },
            { "reload", ("reload [opt: all]", ChatCommandConditions.None, Reload) },
            { "start", (null, ChatCommandConditions.InBattle, Start) },
            { "shutdown", (null, ChatCommandConditions.Admin, Shutdown) },
            { "supplyrain", (null, ChatCommandConditions.ActiveBattle, SupplyRain) }
        };

        public static void CheckForCommand(string command, Player player)
        {
            if (!player.Data.Admin) return;
            string[] args = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!Commands.ContainsKey(args[0].ToLower())) return;

            (string, ChatCommandConditions, Func<Player, string[], string>) desc = Commands[args[0].ToLower()];
            ChatCommandConditions playerConditions = ChatCommands.GetConditionsFor(player);

            foreach (ChatCommandConditions condition in Enum.GetValues<ChatCommandConditions>())
            {
                if ((desc.Item2 & condition) != condition || (playerConditions & condition) == condition) continue;
                CommandReply(ChatCommands.ConditionErrors[condition], player);
                return;
            }

            CommandReply(desc.Item3(player, args[1..]), player);
        }

        private static string ChangeBattleMode(Player player, string[] args)
        {
            string prevMode = player.BattlePlayer.Battle.Params.BattleMode.ToString();
            ClientBattleParams newParams = player.BattlePlayer.Battle.Params;
            string toConvertMode = args.Length == 0 ? "" : args[0];

            newParams.BattleMode = toConvertMode.ToLower() switch
            {
                "ctf" => BattleMode.CTF,
                "dm" => BattleMode.DM,
                "tdm" => BattleMode.TDM,
                _ => Battles.Matchmaking.MatchMaking.BattleModePicker()
            };
            player.BattlePlayer.Battle.UpdateParams(newParams);

            return
                $"Successfully changed battle mode from {prevMode} to {player.BattlePlayer.Battle.Params.BattleMode.ToString()}";
        }

        private static string ChangeFriendlyFire(Player player, string[] args)
        {
            ClientBattleParams newParams = player.BattlePlayer.Battle.Params;
            newParams.FriendlyFire = !newParams.FriendlyFire;
            player.BattlePlayer.Battle.UpdateParams(newParams);

            return $"{(newParams.FriendlyFire ? "Activated" : "Deactivated")} friendly fire";
        }

        private static string ChangeMap(Player player, string[] args)
        {
            ClientBattleParams newParams = player.BattlePlayer.Battle.Params;
            List<MapInfo> otherMaps = ServerConnection.ServerMapInfo.Values
                .Where(mapInfo => mapInfo.MapId != player.BattlePlayer.Battle.Params.MapId).ToList();

            string prevMapName = player.BattlePlayer.Battle.CurrentMapInfo.Name;
            string newMapName = args.Length == 0 ? otherMaps[new Random().Next(otherMaps.Count)].Name : args[0];
            long? newMapId = ServerConnection.ServerMapInfo.Values.SingleOrDefault(m =>
                string.Equals(m.Name, newMapName, StringComparison.CurrentCultureIgnoreCase))?.MapId;

            if (newMapId != null)
            {
                newParams.MapId = (long) newMapId;
                player.BattlePlayer.Battle.UpdateParams(newParams);
            }
            else
                return $"Error, a map with name {newMapName} wasn't found";

            return $"Successfully changed battle map from {prevMapName} to {player.BattlePlayer.Battle.CurrentMapInfo.Name}";
        }

        private static string ChangeModules(Player player, string[] args)
        {
            ClientBattleParams newParams = player.BattlePlayer.Battle.Params;
            newParams.DisabledModules = !newParams.DisabledModules;
            player.BattlePlayer.Battle.UpdateParams(newParams);

            return $"{(newParams.DisabledModules ? "Deactivated" : "Activated")} modules";
        }

        private static void CommandReply(string message, Player player) =>
            player.ShareEntities(SimpleTextNotificationTemplate.CreateEntity(
                $"Command{(message.StartsWith("error") ? "" : ":")} {message}"));

        private static string DailyBonusRecharge(Player player, string[] args)
        {
            bool alreadyAvailable = player.Data.DailyBonusNextReceiveDate <= DateTime.UtcNow;
            player.Data.DailyBonusNextReceiveDate = DateTime.UtcNow;

            return alreadyAvailable
                ? "daily bonus is already available"
                : "recharged teleportation device";
        }


        private static IEnumerable<Battle> FindTargetedBattles(string targetName, Player player)
        {
            List<Battle> targets = new();
            switch (targetName)
            {
                case "all":
                    targets.AddRange(ServerConnection.BattlePool);
                    break;
                case "custom":
                    targets.AddRange(ServerConnection.BattlePool.Where(b => !b.IsMatchMaking));
                    break;
                case "mm":
                    targets.AddRange(ServerConnection.BattlePool.Where(b => b.IsMatchMaking));
                    break;
                case "others":
                    targets.AddRange(
                        ServerConnection.BattlePool.Where(b => b.FindPlayerByUsername(player.Data.Username) == null));
                    break;
                case "this" or "":
                    if (player.IsInBattle)
                        targets.Add(player.BattlePlayer.Battle);
                    break;
            }
            return targets;
        }


        private static string Finish(Player player, string[] args)
        {
            string targetName = args.Length != 0 ? args[0] : "";
            IEnumerable<Battle> targets = FindTargetedBattles(targetName, player);

            int counter = 0;
            foreach (Battle battle in targets)
            {
                battle.FinishBattle();
                counter++;
            }

            return $"Force finished {counter} battle{(counter is > 1 or 0 ? "s" : "")}";
        }

        private static string FractionsCompetitionEditor(Player player, string[] args)
        {
            string message;

            switch (args[0])
            {
                case "addScore":
                    if (!player.ServerData.FractionsCompetitionActive)
                        return "error: fraction competition needs to be active";
                    if (args.Length < 3) return "Error: fraction name or additional score argument is missing";
                    if (!long.TryParse(args[2], out long additionalScore))
                        return $"error: unable to parse '{args[2]}' as score";

                    switch (args[1].ToLower())
                    {
                        case "antaeus":
                            player.ServerData.AntaeusScore += additionalScore;
                            break;
                        case "frontier":
                            player.ServerData.FrontierScore += additionalScore;
                            break;
                        default:
                            return $"error: couldn't find fraction '{args[1]}'";
                    }

                    foreach (Player p in Server.Instance.Connection.Pool.Where(p => p.User is not null).ToList())
                        p.UpdateFractionScores();

                    return $"Added {additionalScore} score points to the " +
                           $"{new CultureInfo("en-US").TextInfo.ToTitleCase(args[1])} fraction";
                case "finish":
                    if (player.ServerData.FractionsCompetitionFinished)
                        return "error: fractions competition is already finished";
                    player.ServerData.FractionsCompetitionFinished = true;
                    message = "fractions competition has been finished";
                    break;
                case "reset":
                    // todo: reset user fraction score of every user in database
                    player.ServerData.FractionsCompetitionActive = false;
                    player.ServerData.FractionsCompetitionFinished = false;
                    player.ServerData.AntaeusScore = 0;
                    player.ServerData.AntaeusUserCount = 0;
                    player.ServerData.FrontierScore = 0;
                    player.ServerData.FrontierUserCount = 0;
                    message = "all fractions competition values have been set to default";
                    break;
                case "start":
                    if (player.ServerData.FractionsCompetitionActive)
                        return "error: fractions competition is already active";
                    player.ServerData.FractionsCompetitionActive = true;
                    message = "fractions competition has been started";
                    break;
                default:
                    return $"error: argument {args[0]} isn't valid";
            }

            PropertyInfo info = typeof(Fractions).GetProperty("GlobalItems");
            Entity[] fractionItems = ((ItemList) info?.GetValue(null))?.GetAllItems();

            foreach (Player p in Server.Instance.Connection.Pool.Where(p => p.User is not null))
            {
                p.UnshareEntities(p.EntityList.Where(e =>
                    !string.IsNullOrEmpty(e.TemplateAccessor.ConfigPath) &&
                    e.TemplateAccessor.ConfigPath.StartsWith("fractionscompetition")));
                p.ShareEntities(fractionItems);
                p.UpdateFractionScores();
            }

            return message;
        }

        private static string GoldboxRain(Player player, string[] args)
        {
            Battle battle = player.BattlePlayer.Battle;
            IEnumerable<BattleBonus> unusedGolds = battle.GoldBonuses
                .Where(b => b.State == BonusState.Unused).ToArray();

            if (args.Length != 0)
            {
                bool successfullyParsed = int.TryParse(args[0], out int amount);
                if (!successfullyParsed) return "error, unable to parse amount of goldboxes";
                if (unusedGolds.Count() < amount)
                    return $"error, too few goldboxes ({unusedGolds.Count()} are available)";
                Random random = new();
                unusedGolds = unusedGolds.OrderBy(_ => random.Next()).Take(amount);
            }

            int counter = 0;
            foreach (BattleBonus goldBonus in unusedGolds)
            {
                goldBonus.CurrentCrystals = 0;
                goldBonus.State = BonusState.New;
                battle.PlayersInMap.SendEvent(new GoldScheduleNotificationEvent(""), battle.RoundEntity);
                counter++;
            }

            return counter > 0
                ? $"Get an umbrella, {counter} goldboxes will be dropped soon"
                : "error, no supplies are ready to drop";
        }

        private static string Immune(Player player, string[] args)
        {
            player.BattlePlayer.IsCheatImmune = !player.BattlePlayer.IsCheatImmune;
            return player.BattlePlayer.IsCheatImmune
                ? "Congrats, you got vaccinated"
                : "You got successfully unvaccinated";
        }

        private static string Message(Player player, string[] args)
        {
            List<Player> targets = new();
            switch (args[0])
            {
                case "all":
                    targets = player.Server.Connection.Pool;
                    break;
                default:
                    Player target = player.Server.FindPlayerByUsername(args[0]);
                    if (target is null) return $"error: couldn't find target '{args[0]}'";
                    targets.Add(target);
                    break;
            }

            Entity notification = SimpleTextNotificationTemplate.CreateEntity(string.Join(" ", args[1..]));
            targets.ShareEntities(notification);

            return $"Sent message to {targets.Count} target{(targets.Count > 1 ? "s" : "")}";
        }

        private static string Open(Player player, string[] args)
        {
            string targetName = args.Length != 0 ? args[0] : "";
            IEnumerable<Battle> targets = FindTargetedBattles(targetName, player);

            int counter = 0;
            foreach (Battle battle in targets)
            {
                if (!battle.ForceOpen) counter++;
                battle.ForceOpen = true;
                (battle.TypeHandler as Battle.CustomBattleHandler)?.OpenBattle();
            }

            return $"Force opened {counter} battle{(counter is > 1 or 0 ? "s" : "")}";
        }

        private static string Pause(Player player, string[] args)
        {
            string targetName = args.Length != 0 ? args[0] : "";
            IEnumerable<Battle> targets = FindTargetedBattles(targetName, player);

            int counter = 0;
            foreach (Battle battle in targets)
            {
                if (!battle.ForceStart) counter++;
                battle.ForceStart = false;
                battle.ForcePause = true;
            }

            return $"Switched force pause state for {counter} battle{(counter is > 1 or 0 ? "s" : "")}";
        }

        private static string PositionInfo(Player player, string[] args)
        {
            MatchPlayer matchPlayer = player.BattlePlayer.MatchPlayer;
            return $"Vector3: {matchPlayer.TankPosition} || Quaternion: {matchPlayer.TankQuaternion}";
        }

        private static string RecruitReward(Player player, string[] args)
        {
            switch (args[0])
            {
                case "reset":
                    player.Data.RecruitRewardDay = 0;
                    return "recruit reward days counter has been reset";
                case "skip":
                    player.Data.LastRecruitReward = DateTimeOffset.MinValue;
                    return "skipped recruit reward waiting time";
                case "check" or _:
                    player.CheckRecruitReward();
                    return $"checked reward necessity. Recruit reward day: {player.Data.RecruitRewardDay}";
            }
        }

        private static string Reload(Player player, string[] args)
        {
            List<BaseBattlePlayer> targets = new();
            string targetName = args.Any() ? args[0] : player.Data.Username;
            switch (targetName)
            {
                case "all":
                    if (!player.IsInMatch) return "Error: Target 'all' is only possible in a match";
                    targets.AddRange(player.BattlePlayer.Battle.PlayersInMap);
                    break;
                default:
                    if (!ModCommands.FindPlayer(targetName, out Player target, out string error)) return error;
                    if (!target.IsInMatch)
                        return $"Error: Player '{target.Data.Username}' isn't in a match";
                    targets.Add(target.BattlePlayer);
                    break;
            }

            foreach (BaseBattlePlayer battlePlayer in targets)
            {
                battlePlayer.Battle.KeepRunning = true;
                battlePlayer.Rejoin = true;
                battlePlayer.SendEvent(new KickFromBattleEvent(), (battlePlayer as BattleTankPlayer)?.MatchPlayer.BattleUser ?? ((Spectator)battlePlayer).BattleUser);
            }

            return $"Target{(targets.Count > 1 ? "s" : "")} rejoin{(targets.Count < 2 ? "s" : "")}";
        }

        private static string Shutdown(Player player, string[] args)
        {
            Server.Instance.Stop();
            return "Senseless message: stopped server";
        }

        private static string Start(Player player, string[] args)
        {
            string targetName = args.Length != 0 ? args[0] : "";
            IEnumerable<Battle> targets = FindTargetedBattles(targetName, player);

            int counter = 0;
            foreach (Battle battle in targets)
            {
                if (!battle.IsMatchMaking) battle.BattleState = BattleState.Starting;
                if (!battle.ForceStart) counter++;
                battle.ForcePause = false;
                battle.ForceStart = true;
            }

            return $"Force started {counter} battle{(counter is > 1 or 0 ? "s" : "")}";
        }

        private static string SupplyRain(Player player, string[] args)
        {
            if (player.BattlePlayer.Battle.Params.DisabledModules) return "error, modules & supplies are disabled";

            int counter = 0;
            foreach (BattleBonus battleBonus in player.BattlePlayer.Battle.BattleBonuses)
            {
                battleBonus.StateChangeCountdown = 20;
                counter++;
            }

            return counter > 0
                ? $"Get an umbrella, {counter} supplies will be dropped soon"
                : "error, no supplies are ready to drop";
        }
    }
}
