using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using System;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Components;
using System.Linq;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Components.Battle;
using System.Numerics;
using System.Collections.Generic;
using TXServer.Core.Battles;
using TXServer.ECSSystem.EntityTemplates.Battle;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1463741053998L)]
    public class FlagCollisionRequestEvent : ECSEvent
    {
        public static void Execute(Player player, Entity tank, Entity flag)
        {
            Core.Battles.Battle battle = ServerConnection.BattlePool.First(b => b.BattleEntity.GetComponent<BattleGroupComponent>().Key == tank.GetComponent<BattleGroupComponent>().Key);
            Entity[] flags = { battle.BlueFlagEntity, battle.RedFlagEntity };
            Entity enemyFlag = flags.Single(f => f.GetComponent<TeamGroupComponent>().Key != tank.GetComponent<TeamGroupComponent>().Key);
            Entity allieFlag = flags.Single(f => f.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key);

            // enemy team flag
            if (flag.GetComponent<TeamGroupComponent>().Key != tank.GetComponent<TeamGroupComponent>().Key)
            {
                // capture enemy flag
                if (flag.GetComponent<FlagGroundedStateComponent>() == null && flag.GetComponent<TankGroupComponent>() == null)
                {
                    battle.FlagBlockedTankKey = tank.GetComponent<TankGroupComponent>().Key;
                    CommandManager.SendCommands(player,
                        new ComponentAddCommand(flag, new TankGroupComponent(tank)),
                        new ComponentRemoveCommand(flag, typeof(FlagHomeStateComponent)));
                    CommandManager.BroadcastCommands(battle.RedTeamPlayers.Concat(battle.BlueTeamPlayers).Select(x => x.Player),
                        new SendEventCommand(new FlagPickupEvent(), flag));
                }
                // pickup enemy flag
                else
                {
                    if (tank.GetComponent<TankGroupComponent>().Key != battle.FlagBlockedTankKey)
                    {
                        battle.FlagBlockedTankKey = tank.GetComponent<TankGroupComponent>().Key;
                        CommandManager.SendCommands(battle.BlueTeamPlayers[0].Player,
                            new ComponentRemoveCommand(flag, typeof(TankGroupComponent)),
                            new ComponentAddCommand(flag, new TankGroupComponent(tank)));
                        CommandManager.BroadcastCommands(battle.RedTeamPlayers.Concat(battle.BlueTeamPlayers).Select(x => x.Player),
                            new SendEventCommand(new FlagPickupEvent(), flag));
                    }
                    else
                    {
                        battle.FlagBlockedTankKey = null;
                    }
                }
            } 
            // allie team flag
            else {
                // return allie flag
                if (allieFlag.GetComponent<TankGroupComponent>() != null)
                {
                    CommandManager.SendCommands(player,
                        new ComponentRemoveCommand(flag, typeof(TankGroupComponent)),
                        new ComponentAddCommand(flag, new TankGroupComponent(tank)),
                        new ComponentRemoveCommand(flag, typeof(FlagGroundedStateComponent)));
                    // STORMTROOPER ROCKS
                    Entity newFlag;
                    if (battle.RedFlagEntity.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key)
                    {
                        battle.RedFlagEntity = FlagTemplate.CreateEntity(battle.MapCoordinates.flags.flagRed.position.V3, team: battle.RedTeamEntity, battle: battle.BattleEntity);
                        newFlag = battle.RedFlagEntity;
                    }
                    else
                    {
                        battle.BlueFlagEntity = FlagTemplate.CreateEntity(battle.MapCoordinates.flags.flagBlue.position.V3, team: battle.BlueTeamEntity, battle: battle.BattleEntity);
                        newFlag = battle.BlueFlagEntity;
                    }
                    CommandManager.BroadcastCommands(battle.RedTeamPlayers.Concat(battle.BlueTeamPlayers).Select(x => x.Player),
                        new SendEventCommand(new FlagReturnEvent(), flag),
                        new EntityUnshareCommand(flag),
                        new EntityShareCommand(newFlag));
                } 
                // deliver enemy flag
                else
                {
                    if (enemyFlag.GetComponent<TankGroupComponent>() != null)
                    {
                        if (enemyFlag.GetComponent<TankGroupComponent>().Key == tank.GetComponent<TankGroupComponent>().Key)
                        {
                            Entity newFlag;
                            if (battle.RedFlagEntity.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key)
                            {
                                battle.BlueFlagEntity = FlagTemplate.CreateEntity(battle.MapCoordinates.flags.flagBlue.position.V3, team: battle.BlueTeamEntity, battle: battle.BattleEntity);
                                newFlag = battle.BlueFlagEntity;
                            }
                            else
                            {
                                battle.RedFlagEntity = FlagTemplate.CreateEntity(battle.MapCoordinates.flags.flagRed.position.V3, team: battle.RedTeamEntity, battle: battle.BattleEntity);
                                newFlag = battle.RedFlagEntity;
                            }
                            CommandManager.BroadcastCommands(battle.RedTeamPlayers.Concat(battle.BlueTeamPlayers).Select(x => x.Player),
                                new SendEventCommand(new FlagDeliveryEvent(), enemyFlag),
                                new EntityUnshareCommand(enemyFlag),
                                new EntityShareCommand(newFlag));
                        }
                    }
                }
            }
        }
    }
}