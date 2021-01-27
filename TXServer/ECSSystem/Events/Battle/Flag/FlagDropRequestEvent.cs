using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-1910863908782544246L)]
    public class FlagDropRequestEvent : ECSEvent
    {
        public static void Execute(Player player, Entity flag, Entity tank)
        {
            Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.BattleEntity.GetComponent<BattleGroupComponent>().Key == tank.GetComponent<BattleGroupComponent>().Key);
            
            battle.FlagBlockedTankKey = tank.GetComponent<TankGroupComponent>().Key;
            Entity[] flags = { battle.BlueFlagEntity, battle.RedFlagEntity };
            Entity enemyFlag = flags.Single(f => f.GetComponent<TeamGroupComponent>().Key != tank.GetComponent<TeamGroupComponent>().Key);

            List<ICommand> commands = new List<ICommand>
            {
                new ComponentAddCommand(enemyFlag, new FlagGroundedStateComponent()),
                // TODO: drop flag at latest tank position
                new ComponentChangeCommand(enemyFlag, new FlagPositionComponent(new Vector3(x: 0, y: 3, z: 0)))
            };
            if (enemyFlag.GetComponent<FlagGroundedStateComponent>() != null)
            {
                commands.Insert(0, new ComponentRemoveCommand(enemyFlag, typeof(FlagGroundedStateComponent)));
            }
            
            CommandManager.SendCommands(player,
               commands);
            CommandManager.BroadcastCommands(battle.RedTeamPlayers.Concat(battle.BlueTeamPlayers).Select(x => x.Player),
                new SendEventCommand(new FlagDropEvent(IsUserAction: true), enemyFlag));
        }
    }
}