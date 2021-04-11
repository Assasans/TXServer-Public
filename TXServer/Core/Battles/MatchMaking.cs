using System.Linq;
using TXServer.Core.Squads;
using TXServer.ECSSystem.Components.Battle.Time;

namespace TXServer.Core.Battles
{
    public static class MatchMaking
    {
        public static void FindBattle(Player player)
        {
            Battle battle = ServerConnection.BattlePool.OrderBy(b => b.AllBattlePlayers).LastOrDefault(IsValidToEnter) ??
                            CreateMatchMakingBattle();

            battle.AddPlayer(player, false);
        }

        public static void FindSquadBattle(Squad squad)
        {
            Battle battle = ServerConnection.BattlePool.OrderBy(b => b.AllBattlePlayers).LastOrDefault(b => IsValidForSquad(b, squad)) ??
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
            if (!battle.IsMatchMaking || battle.AllBattlePlayers.Count() == battle.Params.MaxPlayers || battle.BattleState == BattleState.Ended)
                return false;

            if (battle.BattleState == BattleState.Running)
            {
                return battle.CountdownTimer >=
                    battle.BattleEntity.GetComponent<TimeLimitComponent>().TimeLimitSec - 180 || battle.ForceOpen;
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
                    Battle.TeamBattleHandler teamHandler = handler;

                    if (teamHandler.RedTeamPlayers.Count <= battle.Params.MaxPlayers / 2 - squad.Participants.Count)
                        return true;
                    if (teamHandler.BlueTeamPlayers.Count <= battle.Params.MaxPlayers / 2 - squad.Participants.Count)
                        return true;
                    break;
                }
                case Battle.DMHandler dmHandler when dmHandler.Players.Count() <= battle.Params.MaxPlayers - squad.Participants.Count:
                    return true;
            }
            return false;
        }
    }
}