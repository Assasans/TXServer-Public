using System;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Matchmaking
{
    [SerialVersionUID(1496829083447)]
    public class MatchMakingUserReadyEvent : ECSEvent
    {
        public void Execute(Player player, Entity lobby)
        {
            //todo handle it in the lobby
            CommandManager.SendCommands(player, new ComponentAddCommand(player.User, new MatchMakingUserReadyComponent()));

            Core.Battles.Battle battle = ServerConnection.BattlePool.SingleOrDefault(b => b.BlueTeamPlayers.Concat(b.RedTeamPlayers).Concat(b.DMTeamPlayers).Contains(player.BattleLobbyPlayer));
            if (battle.BattleState == BattleState.Running)
            {
                BattleLobbyPlayer toRemoveBattleLobbyPlayer = battle.WaitingToJoinPlayers.SingleOrDefault(p => p.BattlePlayer == player.BattleLobbyPlayer.BattlePlayer);
                if (toRemoveBattleLobbyPlayer != null)
                {
                    battle.WaitingToJoinPlayers.Remove(toRemoveBattleLobbyPlayer);
                }

                battle.InitBattlePlayer(player.BattleLobbyPlayer);
            }
        }
    }
}