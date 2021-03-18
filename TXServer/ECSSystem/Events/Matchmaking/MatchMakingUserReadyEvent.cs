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
            //todo handle it in the lobby
            player.User.AddComponent(new MatchMakingUserReadyComponent());

            Core.Battles.Battle battle = player.BattlePlayer.Battle;
            if (battle.BattleState == BattleState.Running)
                player.BattlePlayer.MatchMakingJoinCountdown = DateTime.Now.AddSeconds(2);
        }
    }
}