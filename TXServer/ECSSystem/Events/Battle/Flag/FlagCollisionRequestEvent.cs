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
            FlagState flagState = battle.FlagStates[flag];

            if (battle.BattleState == BattleState.WarmUp)
                return;

            switch (flagState)
            {
                case FlagState.Home:
                    if (flag.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key)
                    {
                        // deliver enemy flag
                        Entity[] flags = { battle.BlueFlagEntity, battle.RedFlagEntity };
                        Entity enemyFlag = flags.Single(f => f.GetComponent<TeamGroupComponent>().Key != tank.GetComponent<TeamGroupComponent>().Key);
                        
                        if (enemyFlag.GetComponent<TankGroupComponent>() != null) {
                             }

                        if (battle.FlagStates[enemyFlag] == FlagState.Captured)
                        {
                            if (enemyFlag.GetComponent<TankGroupComponent>().Key != tank.GetComponent<TankGroupComponent>().Key)
                                return;
                        }
                        else return;

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

                        battle.FlagStates.Remove(enemyFlag);
                        battle.FlagStates.Add(newFlag, FlagState.Home);
                        enemyFlag.ChangeComponent(new FlagPositionComponent(alliePedestal.GetComponent<FlagPedestalComponent>().Position));
                        enemyFlag.AddComponent(new FlagGroundedStateComponent());
                        battle.AllBattlePlayers.Select(x => x.Player).SendEvent(new FlagDeliveryEvent(), enemyFlag);
                        battle.AllBattlePlayers.Select(x => x.Player).UnshareEntity(enemyFlag);
                        battle.AllBattlePlayers.Select(x => x.Player).ShareEntity(newFlag);

                        battle.UpdateScore(player, allieTeam, 1);
                        battle.UpdateUserStatistics(player, additiveScore: enemyPlayers * 10, additiveKills: 0, additiveKillAssists: 0, additiveDeath: 0);
                    }
                    else
                    {
                        // capture enemy flag
                        // TODO: calculate points
                        battle.FlagStates[flag] = FlagState.Captured;
                        flag.AddComponent(new TankGroupComponent(tank));
                        flag.RemoveComponent<FlagHomeStateComponent>();
                        battle.AllBattlePlayers.Select(x => x.Player).SendEvent(new FlagPickupEvent(), flag);
                    }
                    break;
                case FlagState.Dropped:
                    if (flag.GetComponent<TeamGroupComponent>().Key == tank.GetComponent<TeamGroupComponent>().Key)
                    {
                        // return allie flag
                        // TODO: calculate points
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

                        battle.DroppedFlags.Remove(flag);
                        battle.FlagStates.Remove(flag);
                        battle.FlagStates.Add(newFlag, FlagState.Home);
                        flag.RemoveComponent<TankGroupComponent>();
                        flag.AddComponent(new TankGroupComponent(tank));
                        flag.RemoveComponent<FlagGroundedStateComponent>();
                        battle.AllBattlePlayers.Select(x => x.Player).SendEvent(new FlagReturnEvent(), flag);
                        battle.AllBattlePlayers.Select(x => x.Player).UnshareEntity(flag);
                        battle.AllBattlePlayers.Select(x => x.Player).ShareEntity(newFlag);
                    }
                    else
                    {
                        // pickup flag
                        if (player.BattleLobbyPlayer.BattlePlayer.FlagBlocks > 0)
                        {
                            player.BattleLobbyPlayer.BattlePlayer.FlagBlocks -= 1;
                            return;
                        }
                        battle.DroppedFlags.Remove(flag);
                        battle.FlagStates[flag] = FlagState.Captured;
                        flag.RemoveComponent<TankGroupComponent>();
                        flag.AddComponent(new TankGroupComponent(tank));
                        flag.RemoveComponent<FlagGroundedStateComponent>();
                        battle.AllBattlePlayers.Select(x => x.Player).SendEvent(new FlagPickupEvent(), flag);
                    }
                    break;
            }
        }
    }
}