using System;
using TXServer.Core;
using TXServer.Core.Battles;
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
            player.User.AddComponent(new MatchMakingUserReadyComponent());

            if (player.BattlePlayer.Battle.BattleState == BattleState.Running)
                player.BattlePlayer.MatchMakingJoinCountdown = DateTime.UtcNow.AddSeconds(2);
        }
    }
}
