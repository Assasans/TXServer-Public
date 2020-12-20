using System;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(6959116100408127452)]
    public class MoveCommandEvent : ECSEvent
    {
        public MoveCommand MoveCommand { get; set; }

        public void Execute(Player player, Entity tank)
        {
            player.BattleLobbyPlayer.BattlePlayer.LastMoveCommand = MoveCommand;
        }
    }
}