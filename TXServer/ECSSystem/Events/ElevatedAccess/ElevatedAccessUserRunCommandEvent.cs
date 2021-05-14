using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Battle.Bonus;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
    [SerialVersionUID(1503493668447L)]
    public class ElevatedAccessUserRunCommandEvent : ECSEvent
    {
        private static readonly Dictionary<string, Action<Player, string[]>> Commands = new()
        {
            { "finish", Finish },
            { "goldrain", GoldboxRain },
            { "immune", Immune },
            { "open", Open },
            { "pause", Pause },
            { "positioninfo", PositionInfo },
            { "start", Start },
            { "shutdown", Shutdown },
            { "supplyrain", SupplyRain }
        };

        public void Execute(Player player, Entity entity)
        {
            if (!player.Data.Admin) return;

            string[] args = Command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (Commands.ContainsKey(args[0].ToLower()))
                Commands[args[0].ToLower()](player, args[1..]);
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
        {
            player.BattlePlayer.IsCheatImmune = !player.BattlePlayer.IsCheatImmune;
        }

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
        {
            Server.Instance.Stop();
        }

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


        public string Command { get; set; }
    }
}
