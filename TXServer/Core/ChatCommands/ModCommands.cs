using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Chat;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.ChatCommands
{
    public static class ModCommands
    {
        private static readonly Dictionary<string, (string, Func<Player, string[], string>)> Commands = new()
        {
            { "ban", ("ban [username/uid] [min/h/d/m/y] [opt: reason]", Ban) },
            { "help", ("help", Help) },
            { "kick", ("kick [username/uid] [opt: location]", Kick) },
            { "list", ("list [username/uid]", ListPunishments) },
            { "mute", ("mute [username/uid] [min/h/d/m/y] [opt: reason]", Mute) },
            { "unban", ("unban [username/uid] [opt: reason]", Unban) },
            { "unmute", ("unmute [username/uid] [opt: reason]", Unmute) },
            { "unwarn", ("unwarn [username/uid] [opt: reason]", Unwarn) },
            { "warn", ("warn [username/uid] [opt: reason]", Warn) }
        };

        public static bool CheckForCommand(Player player, string message, out string reply)
        {
            reply = null;
            if (!message.StartsWith('!')) return false;
            string[] args = message[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0) return false;

            reply = Commands.TryGetValue(args[0], out var desc)
                ? desc.Item2(player, args[1..])
                : "Unknown command. Enter \"/help\" to view available commands";

            return true;
        }


        public static string Ban(Player player, string[] args)
        {
            if (args.Length < 2) return "Error, at least 2 arguments are needed for banning";

            if (!FindPlayer(args[0], out Player target, out string error)) return error;
            if (!GetPunishmentTime(args[1], out DateTimeOffset? endTime, out bool isPermanent,
                out string timeResult, out Dictionary<string, string> trTimes)) return timeResult;
            string reason = args.Length > 2 ? string.Join(" ", args[2..]) : null;

            target.Data.Punishments.Add(new Punishment(PunishmentType.Ban, target, reason,
                trTimes, endTime, isPermanent));
            target.Dispose();

            GetPunishmentMsgData(target, out Entity chat, out IEnumerable<Player> msgTargets);
            ChatMessageReceivedEvent.PunishMessageOtherPlayers(target.Data.Punishments.Last(), msgTargets,
                chat, player);

            return target.Data.Punishments.Last().CreatePunishmentMsg(player);
        }

        private static string Help(Player player, string[] args) =>
            '/' + string.Join("\n/", from command in Commands
                select command.Value.Item1 ?? command.Key);

        private static bool FindPlayer(string targetIdentifier, out Player target, out string error)
        {
            error = $"Command error, couldn't find player '{targetIdentifier}'";
            target = Server.Instance.FindPlayerByUsername(targetIdentifier);
            if (target == null && long.TryParse(targetIdentifier, out long targetUid))
                target = Server.Instance.FindPlayerByUid(targetUid);

            if (target != null && target.Data.Admin)
            {
                error = $"Error, '{target.Data.Username}' is an admin";
                target = null;
            }

            return target != null;
        }

        private static bool ForceRemovePunishments(PunishmentType type, string reason, Player target,
            out Punishment removedPunishment)
        {
            removedPunishment = null;
            bool removed = false;
            foreach (Punishment punishment in target.Data.Punishments.Where(p => p.Type == type && p.IsActive))
            {
                punishment.ForceRemoved = true;
                punishment.ForceRemovedReason = reason;
                punishment.ForceRemoveTime = DateTimeOffset.UtcNow;
                removed = true;
                removedPunishment = punishment;
            }
            return removed;
        }

        private static void GetPunishmentMsgData(Player victim, out Entity chat, out IEnumerable<Player> targets)
        {
            if (victim.IsInMatch)
            {
                chat = victim.BattlePlayer.Battle.GeneralBattleChatEntity;
                targets = victim.BattlePlayer.Battle.MatchTankPlayers.Select(p => p.Player);
            }
            else if (victim.IsInBattle)
            {
                chat = victim.BattlePlayer.Battle.BattleLobbyChatEntity;
                targets = victim.BattlePlayer.Battle.JoinedTankPlayers.Select(p => p.Player);
            }
            else
            {
                chat = Chats.GlobalItems.Ru;
                targets = victim.Server.Connection.Pool;
            }
        }

        private static bool GetPunishmentTime(string time, out DateTimeOffset? endTime, out bool isPermanent,
            out string result, out Dictionary<string, string> trTimes)
        {
            result = $"Error, couldn't convert '{time}' to a valid time";
            isPermanent = false;
            trTimes = new Dictionary<string, string>();
            int unspecificDur;
            List<string> ruTimes = new();

            switch (time.ToLower())
            {
                case { } s when s.EndsWith("d") && int.TryParse(time[..^1], out unspecificDur):
                    endTime = DateTimeOffset.UtcNow.AddDays(unspecificDur);
                    trTimes.Add("de", $"{unspecificDur} Tag{(unspecificDur != 1 ? "e" : "")}");
                    trTimes.Add("en", $"{unspecificDur} day{(unspecificDur != 1 ? "s" : "")}");
                    ruTimes = new List<string> {"дней", "день", "дня", "дней"};
                    break;
                case { } s when s.EndsWith("h") && int.TryParse(time[..^1], out unspecificDur):
                    endTime = DateTimeOffset.UtcNow.AddHours(unspecificDur);
                    trTimes.Add("de", $"{unspecificDur} Stunde{(unspecificDur != 1 ? "n" : "")}");
                    trTimes.Add("en", $"{unspecificDur} hour{(unspecificDur != 1 ? "s" : "")}");
                    ruTimes = new List<string> {"часов", "час", "часа", "часов"};
                    break;
                case { } s when s.EndsWith("m") && int.TryParse(time[..^1], out unspecificDur):
                    endTime = DateTimeOffset.UtcNow.AddMonths(unspecificDur);
                    trTimes.Add("de", $"{unspecificDur} Monat{(unspecificDur != 1 ? "e" : "")}");
                    trTimes.Add("en", $"{unspecificDur} month{(unspecificDur != 1 ? "s" : "")}");
                    ruTimes = new List<string> {"месяцев", "месяц", "месяца", "месяцев"};
                    break;
                case { } s when s.EndsWith("min") && int.TryParse(time[..^3], out unspecificDur):
                    endTime = DateTimeOffset.UtcNow.AddMinutes(unspecificDur);
                    trTimes.Add("de", $"{unspecificDur} Minute{(unspecificDur != 1 ? "n" : "")}");
                    trTimes.Add("en", $"{unspecificDur} minute{(unspecificDur > 1 ? "s" : "")}");
                    ruTimes = new List<string> {"минут", "минуту", "минуты", "минут"};
                    break;
                case { } s when s.EndsWith("s") && int.TryParse(time[..^1], out unspecificDur):
                    endTime = DateTimeOffset.UtcNow.AddSeconds(unspecificDur);
                    trTimes.Add("de", $"{unspecificDur} Sekunde{(unspecificDur != 1 ? "n" : "")}");
                    trTimes.Add("en", $"{unspecificDur} second{(unspecificDur != 1 ? "s" : "")}");
                    ruTimes = new List<string> {"секунд", "секунда", "секунды", "секунд"};
                    break;
                case { } s when s.EndsWith("y") && int.TryParse(time[..^1], out unspecificDur):
                    endTime = DateTimeOffset.UtcNow.AddYears(unspecificDur);
                    trTimes.Add("de", $"{unspecificDur} Jahr{(unspecificDur != 1 ? "e" : "")}");
                    trTimes.Add("en", $"{unspecificDur} year{(unspecificDur != 1 ? "s" : "")}");
                    ruTimes = new List<string> {"лет", "год", "года", "лет"};
                    break;
                case "forever":
                    endTime = DateTimeOffset.MaxValue;
                    trTimes.Add("de", "immer");
                    trTimes.Add("en", "ever");
                    isPermanent = true;
                    unspecificDur = Int32.MaxValue;
                    break;
                default:
                    endTime = null;
                    return false;
            }

            string[] ruSpecialNumbers = {"2", "3", "4"};
            trTimes.Add("ru", unspecificDur switch {
                0 => $"{unspecificDur} {ruTimes[0]}",
                1 => $"{unspecificDur} {ruTimes[1]}",
                { } s when ruSpecialNumbers.Any(x => s.ToString().EndsWith(x)) => $"{unspecificDur} {ruTimes[2]}",
                int.MaxValue => "навсегда",
                _ => $"{unspecificDur} {ruTimes[3]}"});
            if (trTimes.Any()) result = trTimes["en"];
            return endTime != null;
        }

        private static string Kick(Player player, string[] args)
        {
            if (args.Length == 0) return "Error, at least 1 argument (target) is needed for kicking";
            if (!FindPlayer(args[0], out Player target, out string error)) return error;

            switch (args.Length > 1 ? args[1] : "")
            {
                case "battle":
                    if (player.IsInBattle && player.IsInMatch)
                    {
                        target.BattlePlayer.Battle.RemovePlayer(target.BattlePlayer);
                        return $"Kicked '{target.Data.Username}' from battle";
                    }
                    else return $"Error, '{target.Data.Username}' isn't in a battle or is already in a match";
                case "match":
                    if (target.IsInMatch)
                    {
                        target.SendEvent(new KickFromBattleEvent());
                        return $"Kicked '{target.Data.Username}' from match";
                    }
                    else return $"Error, '{target.Data.Username}' isn't in a match";
                default:
                    target.Data.SetAutoLogin(false);
                    target.Dispose();
                    return $"Kicked '{target.Data.Username}' from the server";
            }
        }

        private static string ListPunishments(Player player, string[] args)
        {
            if (args.Length == 0) return "Error, at least 1 argument (target) is needed for listing punishments";
            if (!FindPlayer(args[0], out Player target, out string error)) return error;
            string filter = args.Length > 1 ? args[1] : "all";
            string writtenFilter = "";
            List<Punishment> punishments = new();

            switch (filter)
            {
                case "all":
                    punishments.AddRange(target.Data.Punishments);
                    break;
                case "active":
                    punishments.AddRange(target.Data.Punishments.Where(p => p.IsActive));
                    writtenFilter = "active ";
                    break;
                case "inactive":
                    punishments.AddRange(target.Data.Punishments.Where(p => !p.IsActive));
                    writtenFilter = "inactive ";
                    break;
            }

            string listMsg =
                punishments.Aggregate("", (current, punishment) => current + punishment.CreateLogMsg(player));


            return string.IsNullOrEmpty(listMsg) ? $"'{target.Data.Username}' doesn't have any {writtenFilter}punishments" : listMsg;
        }

        public static string Mute(Player player, string[] args)
        {
            if (args.Length < 2) return "Error, at least 2 arguments are needed for muting";

            if (!FindPlayer(args[0], out Player target, out string error)) return error;
            if (!GetPunishmentTime(args[1], out DateTimeOffset? endTime, out bool isPermanent,
                out string timeResult, out Dictionary<string, string> trTimes)) return timeResult;
            string reason = args.Length > 2 ? string.Join(" ", args[2..]) : null;

            target.Data.Punishments.Add(new Punishment(PunishmentType.Mute, target, reason,
                trTimes, endTime, isPermanent));

            GetPunishmentMsgData(target, out Entity chat, out IEnumerable<Player> msgTargets);
            ChatMessageReceivedEvent.PunishMessageOtherPlayers(target.Data.Punishments.Last(), msgTargets,
                chat, player);

            return target.Data.Punishments.Last().CreatePunishmentMsg(player);
        }

        public static string Unban(Player player, string[] args)
        {
            if (args.Length == 0) return "Error, at least 1 argument is needed for unbanning";
            if (!FindPlayer(args[0], out Player target, out string error)) return error;
            string reason = args.Length > 1 ? string.Join(" ", args[1..]) : null;

            if (!ForceRemovePunishments(PunishmentType.Ban, reason, target, out Punishment removedPunishment))
                return $"Error, '{target.Data.Username}' isn't banned";

            GetPunishmentMsgData(target, out Entity chat, out IEnumerable<Player> msgTargets);
            ChatMessageReceivedEvent.PunishMessageOtherPlayers(removedPunishment, msgTargets, chat, player);

            return removedPunishment.CreatePunishmentMsg(player);
        }

        private static string Unmute(Player player, string[] args)
        {
            if (args.Length == 0) return "Error, at least 1 argument is needed for unmuting";
            if (!FindPlayer(args[0], out Player target, out string error)) return error;
            string reason = args.Length > 1 ? string.Join(" ", args[1..]) : null;

            if (!ForceRemovePunishments(PunishmentType.Mute, reason, target, out Punishment removedPunishment))
                return $"Command error, '{target.Data.Username}' isn't banned";

            GetPunishmentMsgData(target, out Entity chat, out IEnumerable<Player> msgTargets);
            ChatMessageReceivedEvent.PunishMessageOtherPlayers(removedPunishment, msgTargets, chat, player);

            return removedPunishment.CreatePunishmentMsg(player);
        }

        private static string Unwarn(Player player, string[] args)
        {
            if (args.Length == 0) return "Error, at least 1 argument is needed for unwarning";
            if (!FindPlayer(args[0], out Player target, out string error)) return error;
            string reason = args.Length > 1 ? string.Join(" ", args[1..]) : null;

            if (!ForceRemovePunishments(PunishmentType.Warn, reason, target, out Punishment removedPunishment))
                return $"Error, '{target.Data.Username}' isn't warned";

            GetPunishmentMsgData(target, out Entity chat, out IEnumerable<Player> msgTargets);
            ChatMessageReceivedEvent.PunishMessageOtherPlayers(
                removedPunishment, msgTargets, chat, player);

            return removedPunishment.CreatePunishmentMsg(player);
        }

        public static string Warn(Player player, string[] args)
        {
            if (args.Length == 0) return "Error, at least 1 argument is needed for warning";
            if (!FindPlayer(args[0], out Player target, out string error)) return error;
            string reason = args.Length > 1 ? string.Join(" ", args[1..]) : null;

            target.Data.Punishments.Add(new Punishment(PunishmentType.Warn, target, reason));

            GetPunishmentMsgData(target, out Entity chat, out IEnumerable<Player> msgTargets);
            ChatMessageReceivedEvent.PunishMessageOtherPlayers(target.Data.Punishments.Last(), msgTargets,
                chat, player);

            return target.Data.Punishments.Last().CreatePunishmentMsg(player);
        }
    }
}
