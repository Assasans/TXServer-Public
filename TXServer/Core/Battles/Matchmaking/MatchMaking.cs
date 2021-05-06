using System;
using System.Linq;
using TXServer.Core.Squads;
using TXServer.ECSSystem.Components.Battle.Time;
using TXServer.ECSSystem.Components.User;

namespace TXServer.Core.Battles.Matchmaking
{
    public static class MatchMaking
    {
        public static void FindBattle(Player player)
        {
            Battle battle = ServerConnection.BattlePool.OrderBy(b => b.JoinedTankPlayers).LastOrDefault(IsValidToEnter) ??
                            CreateMatchMakingBattle();

            battle.AddPlayer(player, false);
        }

        public static void FindSquadBattle(Squad squad)
        {
            Battle battle = ServerConnection.BattlePool.OrderBy(b => b.JoinedTankPlayers).LastOrDefault(b => IsValidForSquad(b, squad)) ??
                            CreateMatchMakingBattle();

            foreach (SquadPlayer participant in squad.Participants)
                battle.AddPlayer(participant.Player, false);
        }


        private static Battle CreateMatchMakingBattle()
        {
            Battle battle = new Battle(battleParams: null, isMatchMaking: true, owner: null);
            ServerConnection.BattlePool.Add(battle);
            return battle;
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


        public static void ProcessDeserterState(Player player, Battle battle)
        {
            if (!battle.IsMatchMaking) return;

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
                        if (component.GoodBattlesInRow == 2)
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
                }
            });
        }
    }
}
