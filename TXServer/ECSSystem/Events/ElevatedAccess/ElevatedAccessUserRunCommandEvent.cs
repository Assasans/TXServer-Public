using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Battle.Bonus;
using TXServer.ECSSystem.Events.Chat;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
    [SerialVersionUID(1503493668447L)]
    public class ElevatedAccessUserRunCommandEvent : ECSEvent
    {
        private static readonly Dictionary<string, (ChatCommandConditions, Action<Player, string[]>)> Commands = new()
        {
            { "battlemode", (ChatCommandConditions.InactiveBattle, ChangeBattleMode) },
            { "finish", (ChatCommandConditions.InBattle, Finish) },
            { "friendlyfire", (ChatCommandConditions.InactiveBattle, ChangeFriendlyFire) },
            { "goldrain", (ChatCommandConditions.ActiveBattle, GoldboxRain) },
            { "immune", (ChatCommandConditions.InBattle, Immune) },
            { "map", (ChatCommandConditions.InactiveBattle, ChangeMap) },
            { "modules", (ChatCommandConditions.InactiveBattle, ChangeModules) },
            { "open", (ChatCommandConditions.InBattle, Open) },
            { "pause", (ChatCommandConditions.InBattle, Pause) },
            { "positioninfo", (ChatCommandConditions.InMatch, PositionInfo) },
            { "start", (ChatCommandConditions.InBattle, Start) },
            { "shutdown", (ChatCommandConditions.Admin, Shutdown) },
            { "supplyrain", (ChatCommandConditions.ActiveBattle, SupplyRain) }
        };

        public void Execute(Player player, Entity session)
        {
            if (!player.Data.Admin) return;

            string[] args = Command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!Commands.ContainsKey(args[0].ToLower())) return;

            (ChatCommandConditions, Action<Player, string[]>) desc = Commands[args[0].ToLower()];
            ChatCommandConditions playerConditions = ChatCommands.GetConditionsFor(player);

            foreach (ChatCommandConditions condition in Enum.GetValues<ChatCommandConditions>())
            {
                if ((desc.Item1 & condition) != condition || (playerConditions & condition) == condition) continue;
                ChatMessageReceivedEvent.SystemMessageTarget(ChatCommands.ConditionErrors[condition], player);
                return;
            }

            desc.Item2(player, args[1..]);
        }

        private static void ChangeBattleMode(Player player, string[] args)
        {
            ClientBattleParams newParams = player.BattlePlayer.Battle.Params;
            string toConvertMode = args.Length == 0 ? "" : args[0];

            newParams.BattleMode = toConvertMode.ToLower() switch
            {
                "ctf" => BattleMode.CTF,
                "dm" => BattleMode.DM,
                "tdm" => BattleMode.TDM,
                _ => Core.Battles.Matchmaking.MatchMaking.BattleModePicker()
            };

            player.BattlePlayer.Battle.UpdateParams(newParams);
        }

        private static void ChangeFriendlyFire(Player player, string[] args)
        {
            ClientBattleParams newParams = player.BattlePlayer.Battle.Params;
            newParams.FriendlyFire = !newParams.FriendlyFire;
            player.BattlePlayer.Battle.UpdateParams(newParams);

            string msg = newParams.FriendlyFire ? "Activated" : "Deactivated";
            ChatMessageReceivedEvent.SystemMessageTarget(msg + " friendly fire",
                player.BattlePlayer.Battle.BattleLobbyChatEntity, player);
        }

        private static void ChangeMap(Player player, string[] args)
        {
            ClientBattleParams newParams = player.BattlePlayer.Battle.Params;
            List<MapInfo> otherMaps = ServerConnection.ServerMapInfo.Values
                .Where(mapInfo => mapInfo.MapId != player.BattlePlayer.Battle.Params.MapId).ToList();

            string newMapName = args.Length == 0 ? otherMaps[new Random().Next(otherMaps.Count)].Name : args[0];
            long? newMapId = ServerConnection.ServerMapInfo.Values.SingleOrDefault(m =>
                string.Equals(m.Name, newMapName, StringComparison.CurrentCultureIgnoreCase))?.MapId;

            if (newMapId != null)
            {
                newParams.MapId = (long) newMapId;
                player.BattlePlayer.Battle.UpdateParams(newParams);
            }
            else
                ChatMessageReceivedEvent.SystemMessageTarget($"Error, a map with name {newMapName} wasn't found",
                    player.BattlePlayer.Battle.BattleLobbyChatEntity, player);
        }

        private static void ChangeModules(Player player, string[] args)
        {
            ClientBattleParams newParams = player.BattlePlayer.Battle.Params;
            newParams.DisabledModules = !newParams.DisabledModules;
            player.BattlePlayer.Battle.UpdateParams(newParams);

            string msg = newParams.DisabledModules ? "Deactivated" : "Activated";
            ChatMessageReceivedEvent.SystemMessageTarget(msg + " modules",
                player.BattlePlayer.Battle.BattleLobbyChatEntity, player);
        }

        private static void Finish(Player player, string[] args)
        {
            string targetName = args.Length != 0 ? args[0] : "";
            IEnumerable<Core.Battles.Battle> targets = FindTargetedBattles(targetName, player);

            foreach (Core.Battles.Battle battle in targets)
                battle.FinishBattle();
        }

        private static void GoldboxRain(Player player, string[] args)
        {
            if (!player.IsInMatch) return;

            Core.Battles.Battle battle = player.BattlePlayer.Battle;
            IEnumerable<BattleBonus> unusedGolds = battle.GoldBonuses
                .Where(b => b.State == BonusState.Unused).ToArray();

            if (args.Length != 0)
            {
                bool successfullyParsed = int.TryParse(args[0], out int amount);
                if (!successfullyParsed) return;
                if (unusedGolds.Count() < amount) return;
                Random random = new();
                unusedGolds = unusedGolds.OrderBy(_ => random.Next()).Take(amount);
            }

            foreach (BattleBonus goldBonus in unusedGolds)
            {
                goldBonus.CurrentCrystals = 0;
                goldBonus.State = BonusState.New;
                battle.PlayersInMap.SendEvent(new GoldScheduleNotificationEvent(""), battle.RoundEntity);
            }
        }

        private static void Immune(Player player, string[] args)
            => player.BattlePlayer.IsCheatImmune = !player.BattlePlayer.IsCheatImmune;


        private static void Open(Player player, string[] args)
        {
            string targetName = args.Length != 0 ? args[0] : "";
            IEnumerable<Core.Battles.Battle> targets = FindTargetedBattles(targetName, player);

            foreach (Core.Battles.Battle battle in targets)
                battle.ForceOpen = true;
        }

        private static void Pause(Player player, string[] args)
        {
            string targetName = args.Length != 0 ? args[0] : "";
            IEnumerable<Core.Battles.Battle> targets = FindTargetedBattles(targetName, player);

            foreach (Core.Battles.Battle unused in targets)
            {
                player.BattlePlayer.Battle.ForceStart = false;
                player.BattlePlayer.Battle.ForcePause = true;
            }
        }

        private static void PositionInfo(Player player, string[] args)
        {
            if (!player.IsInMatch) return;

            MatchPlayer matchPlayer = player.BattlePlayer.MatchPlayer;
            ChatMessageReceivedEvent.SystemMessageTarget($"Vector3: {matchPlayer.TankPosition} || Quaternion: {matchPlayer.TankQuaternion}",
                player.BattlePlayer.Battle.GeneralBattleChatEntity, player);
        }

        private static void Shutdown(Player player, string[] args)
            => Server.Instance.Stop();


        private static void Start(Player player, string[] args)
        {
            string targetName = args.Length != 0 ? args[0] : "";
            IEnumerable<Core.Battles.Battle> targets = FindTargetedBattles(targetName, player);

            foreach (Core.Battles.Battle battle in targets)
            {
                if (!battle.IsMatchMaking)
                    battle.BattleState = BattleState.Starting;
                battle.ForcePause = false;
                battle.ForceStart = true;
            }
        }

        private static void SupplyRain(Player player, string[] args)
        {
            if (player.BattlePlayer.Battle.Params.DisabledModules || !player.IsInMatch) return;

            foreach (BattleBonus battleBonus in player.BattlePlayer.Battle.BattleBonuses)
                battleBonus.StateChangeCountdown = 20;
        }


        private static IEnumerable<Core.Battles.Battle> FindTargetedBattles(string targetName, Player player)
        {
            List<Core.Battles.Battle> targets = new();
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
                        ServerConnection.BattlePool.Where(b => b.FindPlayerByUid(player.Data.Username) == null));
                    break;
                case "this" or "":
                    if (player.IsInBattle)
                        targets.Add(player.BattlePlayer.Battle);
                    break;
            }
            return targets;
        }


        // ReSharper disable once UnassignedGetOnlyAutoProperty
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Command { get; set; }
    }
}
