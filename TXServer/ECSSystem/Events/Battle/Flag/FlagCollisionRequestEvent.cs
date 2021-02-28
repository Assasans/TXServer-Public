using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.Core;
using System.Linq;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.Core.Battles;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1463741053998L)]
    public class FlagCollisionRequestEvent : ECSEvent
    {
        public static void Execute(Player player, Entity tank, Entity flag)
        {
            Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.BattleEntity.GetComponent<BattleGroupComponent>().Key == tank.GetComponent<BattleGroupComponent>().Key);
            if (battle.BattleState == BattleState.WarmingUp)
            {
                return;
            }

            // enemy team flag
            if (flag.GetComponent<TeamGroupComponent>().Key != tank.GetComponent<TeamGroupComponent>().Key)
            {
                // capture enemy flag
                if (flag.GetComponent<FlagHomeStateComponent>() != null)
                {
                    battle.FlagBlockedTankKey = tank.GetComponent<TankGroupComponent>().Key;
                    // TODO: calculate points
                    flag.AddComponent(new TankGroupComponent(tank));
                    flag.RemoveComponent<FlagHomeStateComponent>();

                    foreach (Player player2 in battle.MatchPlayers.Select(x => x.Player))
                        player2.SendEvent(new FlagPickupEvent(), flag);
                }
                // pickup enemy flag
                else if (flag.GetComponent<FlagGroundedStateComponent>() != null)
                {
                    if (battle.FlagBlockedTankKey != tank.GetComponent<TankGroupComponent>().Key)
                    {
                        battle.DroppedFlags.Remove(flag);

                        // TODO: calculate points
                        flag.RemoveComponent<TankGroupComponent>();
                        flag.AddComponent(new TankGroupComponent(tank));
                        flag.RemoveComponent<FlagGroundedStateComponent>();

                        foreach (Player player2 in battle.MatchPlayers.Select(x => x.Player))
                            player2.SendEvent(new FlagPickupEvent(), flag);
                    }
                    else
                    {
                        battle.FlagBlockedTankKey = null;
                    }
                    
                }
            } 
            // allie team flag
            else {

                Entity[] flags = { battle.BlueFlagEntity, battle.RedFlagEntity };
                Entity enemyFlag = flags.Single(f => f.GetComponent<TeamGroupComponent>().Key != tank.GetComponent<TeamGroupComponent>().Key);
                Entity allieFlag = flags.Single(f => f.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key);

                // return allie flag
                if (allieFlag.GetComponent<TankGroupComponent>() != null)
                {
                    battle.DroppedFlags.Remove(flag);

                    flag.RemoveComponent<TankGroupComponent>();
                    flag.AddComponent(new TankGroupComponent(tank));
                    flag.RemoveComponent<FlagGroundedStateComponent>();

                    // STORMTROOPER ROCKS
                    Entity newFlag;
                    if (battle.RedFlagEntity.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key)
                    {
                        battle.RedFlagEntity = FlagTemplate.CreateEntity(battle.CurrentMapInfo.Flags.Red.Position, team: battle.RedTeamEntity, battle: battle.BattleEntity);
                        newFlag = battle.RedFlagEntity;
                    }
                    else
                    {
                        battle.BlueFlagEntity = FlagTemplate.CreateEntity(battle.CurrentMapInfo.Flags.Blue.Position, team: battle.BlueTeamEntity, battle: battle.BattleEntity);
                        newFlag = battle.BlueFlagEntity;
                    }

                    // TODO: calculate points
                    foreach (Player player2 in battle.MatchPlayers.Select(x => x.Player))
                    {
                        player2.SendEvent(new FlagReturnEvent(), flag);
                        player2.UnshareEntity(flag);
                        player2.ShareEntity(newFlag);
                    }
                } 
                // deliver enemy flag
                else
                {
                    if (enemyFlag.GetComponent<TankGroupComponent>() != null)
                    {
                        if (enemyFlag.GetComponent<TankGroupComponent>().Key == tank.GetComponent<TankGroupComponent>().Key)
                        {
                            Entity[] pedestals = { battle.BluePedestalEntity, battle.RedPedestalEntity };
                            Entity alliePedestal = pedestals.Single(p => p.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key);

                            Entity newFlag;
                            Entity allieTeam;
                            int enemyPlayers;
                            if (battle.RedFlagEntity.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key)
                            {
                                battle.BlueFlagEntity = FlagTemplate.CreateEntity(battle.CurrentMapInfo.Flags.Blue.Position, team: battle.BlueTeamEntity, battle: battle.BattleEntity);
                                allieTeam = battle.RedTeamEntity;
                                newFlag = battle.BlueFlagEntity;
                                enemyPlayers = battle.BlueTeamPlayers.Count;
                            }
                            else
                            {
                                battle.RedFlagEntity = FlagTemplate.CreateEntity(battle.CurrentMapInfo.Flags.Red.Position, team: battle.RedTeamEntity, battle: battle.BattleEntity);
                                allieTeam = battle.BlueTeamEntity;
                                newFlag = battle.RedFlagEntity;
                                enemyPlayers = battle.RedTeamPlayers.Count;
                            }
                            int oldScore = allieTeam.GetComponent<TeamScoreComponent>().Score;

                            enemyFlag.ChangeComponent(new FlagPositionComponent(alliePedestal.GetComponent<FlagPedestalComponent>().Position));
                            enemyFlag.AddComponent(new FlagGroundedStateComponent());
                            allieTeam.ChangeComponent(new TeamScoreComponent(++oldScore));

                            foreach (Player player2 in battle.MatchPlayers.Select(x => x.Player))
                            {
                                player2.SendEvent(new FlagDeliveryEvent(), enemyFlag);
                                player2.SendEvent(new RoundScoreUpdatedEvent(), battle.RoundEntity);
                                player2.UnshareEntity(enemyFlag);
                                player2.ShareEntity(newFlag);
                            }

                            battle.UpdatedScore(player);
                            battle.UpdateUserStatistics(player, xp:enemyPlayers*10, kills:0, killAssists:0, death:0);
                        }
                    }
                }
            }
        }
    }
}