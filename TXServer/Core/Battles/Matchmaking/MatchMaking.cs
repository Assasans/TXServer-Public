using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Squads;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Time;
using TXServer.ECSSystem.Components.User;
using TXServer.ECSSystem.Events.Matchmaking;
using TXServer.ECSSystem.Events.MatchMaking;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles.Matchmaking
{
    public static class MatchMaking
    {
        private static void FindOrCreateBattle(Player player)
        {
            Battle battle = ServerConnection.BattlePool.OrderBy(b => b.JoinedTankPlayers.Count()).
                FirstOrDefault(IsValidToEnter) ?? CreateMatchMakingBattle();

            battle.AddPlayer(player, false);
        }

        private static Battle CreateMatchMakingBattle()
        {
            Battle battle = new(null, true, null);
            ServerConnection.BattlePool.Add(battle);
            return battle;
        }

        public static void FindSquadBattle(Squad squad)
        {
            Battle battle = ServerConnection.BattlePool.OrderBy(b => b.JoinedTankPlayers.Count()).
                FirstOrDefault(b => IsValidForSquad(b, squad)) ?? CreateMatchMakingBattle();

            foreach (SquadPlayer participant in squad.Participants)
                battle.AddPlayer(participant.Player, false);
        }

        public static void EnterMatchMaking(Player player, Entity mode = null)
        {
            if (mode is not null)
                player.SendEvent(new EnteredToMatchMakingEvent(), mode);
            WaitingPlayers.Add(player, DateTimeOffset.UtcNow);
        }

        private static bool IsValidToEnter(Battle battle)
        {
            if (!battle.IsMatchMaking || battle.JoinedTankPlayers.Count() == battle.Params.MaxPlayers || battle.BattleState == BattleState.Ended)
                return false;

            if (battle.BattleState == BattleState.Running)
            {
                return battle.CountdownTimer >=
                    battle.BattleEntity.GetComponent<TimeLimitComponent>().TimeLimitSec - 240 || battle.ForceOpen;
            }
            return true;
        }

        private static bool IsValidForSquad(Battle battle, Squad squad)
        {
            bool isValidToEnter = IsValidToEnter(battle);
            if (!isValidToEnter) return false;

            switch (battle.ModeHandler)
            {
                case Battle.TeamBattleHandler handler:
                {
                    if (handler.RedTeamPlayers.Count <= battle.Params.MaxPlayers / 2 - squad.Participants.Count)
                        return true;
                    if (handler.BlueTeamPlayers.Count <= battle.Params.MaxPlayers / 2 - squad.Participants.Count)
                        return true;
                    break;
                }
                case Battle.DMHandler dmHandler when dmHandler.Players.Count() <= battle.Params.MaxPlayers - squad.Participants.Count:
                    return true;
            }
            return false;
        }

        public static void ExitMatchMaking(Player player, Entity matchMaking = null, bool selfAction = true)
        {
            if (player.IsInBattle)
                player.BattlePlayer.WaitingForExit = true;

            if (matchMaking is not null)
                player.SendEvent(new ExitedFromMatchMakingEvent(selfAction), matchMaking);

            if (WaitingPlayers.ContainsKey(player))
                WaitingPlayers.Remove(player);
        }

        public static void ProcessDeserterState(Player player, Battle battle)
        {
            player.User.ChangeComponent<BattleLeaveCounterComponent>(component =>
            {
                if (battle.BattleState == BattleState.Ended)
                {
                    if (component.NeedGoodBattles > 0)
                    {
                        component.NeedGoodBattles--;
                        if (component.NeedGoodBattles == 0) component.Value = 0;
                    }
                    else
                    {
                        if (component.GoodBattlesInRow >= 2)
                        {
                            component.GoodBattlesInRow = 0;
                            if (component.Value != 0 && component.NeedGoodBattles != 0)
                                component.Value--;
                        }
                        else component.GoodBattlesInRow++;
                    }
                }
                else if (battle.EnemyCountFor(player.BattlePlayer) > 0)
                {
                    component.Value++;
                    component.GoodBattlesInRow = 0;
                    if (component.Value >= 2)
                    {
                        if (component.NeedGoodBattles == 0) component.NeedGoodBattles = 3;
                        else component.NeedGoodBattles += (int) component.Value / 2;
                    }

                    player.Data.CurrentBattleSeries = 0;
                }
            });
        }

        public static void Tick()
        {
            foreach ((Player player, DateTimeOffset joinTime) in WaitingPlayers.ToList())
            {
                if (!player.IsActive)
                {
                    WaitingPlayers.Remove(player);
                    continue;
                }
                if (DateTimeOffset.UtcNow <= joinTime.AddSeconds(2)) continue;

                FindOrCreateBattle(player);
                WaitingPlayers.Remove(player);
            }
        }

        public static BattleMode BattleModePicker()
        {
            int percent = new Random().Next(0, 100);
            return percent switch
            {
                < 46 => BattleMode.TDM,
                < 46 + 45 => BattleMode.CTF,
                _ => BattleMode.DM
            };
        }

        private static readonly Dictionary<Player, DateTimeOffset> WaitingPlayers = new();
    }
}
