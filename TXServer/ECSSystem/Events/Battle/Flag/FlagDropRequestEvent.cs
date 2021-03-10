using System;
using System.Linq;
using System.Numerics;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-1910863908782544246L)]
    public class FlagDropRequestEvent : ECSEvent
    {
        public static void Execute(Player player, Entity flag, Entity tank)
        {
            Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.BattleEntity.GetComponent<BattleGroupComponent>().Key == tank.GetComponent<BattleGroupComponent>().Key);

            if (battle.FlagStates[flag] == FlagState.Captured)
                battle.FlagStates[flag] = FlagState.Dropped;
            else
                return;
            battle.DroppedFlags.Add(flag, DateTime.Now.AddMinutes(1));
            player.BattleLobbyPlayer.BattlePlayer.FlagBlocks = 3;
            Vector3 flagPosition = new(player.BattleLobbyPlayer.BattlePlayer.TankPosition.X, player.BattleLobbyPlayer.BattlePlayer.TankPosition.Y - 1, 
                player.BattleLobbyPlayer.BattlePlayer.TankPosition.Z);
            
            CommandManager.SendCommands(player,
                new ComponentAddCommand(flag, new FlagGroundedStateComponent()),
                new ComponentChangeCommand(flag, new FlagPositionComponent(flagPosition)));
            CommandManager.BroadcastCommands(battle.RedTeamPlayers.Concat(battle.BlueTeamPlayers).Select(x => x.Player),
                new SendEventCommand(new FlagDropEvent(IsUserAction: true), flag));
             
        }
    }
}