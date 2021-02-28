﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading;
using TXServer.Core.Commands;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Components.Battle.Time;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Matchmaking;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public class Battle
    {
        public Battle(ClientBattleParams battleParams, bool isMatchMaking, Player owner)
        {
            BattleParams = battleParams;
            IsMatchMaking = isMatchMaking;

            if (isMatchMaking)
            {
                int index = new Random().Next(MatchMakingMaps.Count);
                if (battleParams == null) {
                    battleParams = new ClientBattleParams(BattleMode: BattleMode.CTF, MapId: MatchMakingMaps[index].EntityId, MaxPlayers: 20, TimeLimit: 10, 
                        ScoreLimit: 100, FriendlyFire: false, Gravity: GravityType.EARTH, KillZoneEnabled: true, DisabledModules: false);
                }

                (MapEntity, battleParams.MaxPlayers) = ConvertMapParams(battleParams, isMatchMaking);
                BattleParams = battleParams;
                WarmUpSeconds = 60; // TODO: 1min in Bronze league, 1,5min in Silver, Gold & Master leagues

                BattleLobbyEntity = MatchMakingLobbyTemplate.CreateEntity(battleParams, MapEntity, GravityTypes[battleParams.Gravity]);
            }
            else
            {
                (MapEntity, _) = ConvertMapParams(battleParams, isMatchMaking);
                Owner = owner;
                BattleLobbyEntity = CustomBattleLobbyTemplate.CreateEntity(battleParams, MapEntity, GravityTypes[battleParams.Gravity], owner);

                BattleState = BattleState.CustomNotStarted;
                CountdownTimer = 0;
            }

            BattleEntity = (Entity)BattleEntityCreators[BattleParams.BattleMode].GetMethod("CreateEntity").Invoke(null, new object[] { BattleLobbyEntity, battleParams.ScoreLimit, battleParams.TimeLimit*60, 120 });
            RedTeamEntity = TeamTemplate.CreateEntity(TeamColor.RED, BattleEntity);
            BlueTeamEntity = TeamTemplate.CreateEntity(TeamColor.BLUE, BattleEntity);
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);
            CollisionsComponent = BattleEntity.GetComponent<BattleTankCollisionsComponent>();

            BattleLobbyChatEntity = BattleLobbyChatTemplate.CreateEntity();
            GeneralBattleChatEntity = GeneralBattleChatTemplate.CreateEntity();

            if (BattleParams.BattleMode != BattleMode.DM)
            {
                TeamBattleChatEntity = TeamBattleChatTemplate.CreateEntity();
                if (BattleParams.BattleMode == BattleMode.CTF)
                {
                    RedPedestalEntity = PedestalTemplate.CreateEntity(CurrentMapInfo.Flags.Red.Position, RedTeamEntity, battle: BattleEntity);
                    BluePedestalEntity = PedestalTemplate.CreateEntity(CurrentMapInfo.Flags.Blue.Position, BlueTeamEntity, battle: BattleEntity);
                    RedFlagEntity = FlagTemplate.CreateEntity(CurrentMapInfo.Flags.Red.Position, team: RedTeamEntity, battle: BattleEntity);
                    BlueFlagEntity = FlagTemplate.CreateEntity(CurrentMapInfo.Flags.Blue.Position, team: BlueTeamEntity, battle: BattleEntity);
                }
            }
        }

        public (Entity, int) ConvertMapParams(ClientBattleParams battleParams, bool isMatchMaking)
        {
            Entity[] lowMaxPlayersMaps = new Entity[]
            {
                Maps.GlobalItems.Testbox,
                Maps.GlobalItems.Sandbox,
                Maps.GlobalItems.Area159,
                Maps.GlobalItems.Boombox
            };

            Entity mapEntity = Maps.GlobalItems.Rio;
            int maxPlayers = battleParams.MaxPlayers;
            foreach (PropertyInfo property in typeof(Maps.Items).GetProperties())
            {
                Entity entity = (Entity)property.GetValue(Maps.GlobalItems);
                if (entity.EntityId == battleParams.MapId)
                {
                    mapEntity = entity;
                    CurrentMapInfo = ServerConnection.ServerMapInfo[property.Name];
                    break;
                }
            }

            if (isMatchMaking && lowMaxPlayersMaps.Contains(mapEntity))
                maxPlayers = 8;

            if (battleParams.BattleMode == BattleMode.DM)
            {
                DeathmatchSpawnPoints = CurrentMapInfo.SpawnPoints.Deathmatch;
            }
            else
            {
                var teamModesSpawnPoints = new Dictionary<BattleMode, TeamSpawnPointList>
                {
                    { BattleMode.CTF, CurrentMapInfo.SpawnPoints.CaptureTheFlag },
                    { BattleMode.TDM, CurrentMapInfo.SpawnPoints.TeamDeathmatch }
                };

                TeamSpawnPoints = teamModesSpawnPoints[battleParams.BattleMode];
                // selects the spawnPoints from another team mode if there are no spawn points for the selected one
                if (TeamSpawnPoints == null)
                {
                    TeamSpawnPoints = (TeamSpawnPointList)teamModesSpawnPoints.Where(b => b.Key != battleParams.BattleMode);
                }
            }

            return (mapEntity, maxPlayers);
        }

        public void UpdateBattleParams(Player player, ClientBattleParams battleParams)
        {
            BattleParams = battleParams;
            (MapEntity, _) = ConvertMapParams(battleParams, IsMatchMaking);

            foreach (Component component in new Component[]
            {
                new MapGroupComponent(MapEntity),
                new BattleModeComponent(battleParams.BattleMode),
                new UserLimitComponent(userLimit: battleParams.MaxPlayers, teamLimit: battleParams.MaxPlayers / 2),
                new GravityComponent(gravity: GravityTypes[battleParams.Gravity], gravityType: battleParams.Gravity),
                new ClientBattleParamsComponent(battleParams),
            })
            {
                BattleLobbyEntity.RemoveComponent(component.GetType());
                BattleLobbyEntity.AddComponent(component);
            }

            BattleEntity = (Entity)BattleEntityCreators[BattleParams.BattleMode].GetMethod("CreateEntity").Invoke(null, new object[] { BattleLobbyEntity, battleParams.ScoreLimit, battleParams.TimeLimit*60, 120 });
            RedTeamEntity = TeamTemplate.CreateEntity(TeamColor.RED, BattleEntity);
            BlueTeamEntity = TeamTemplate.CreateEntity(TeamColor.BLUE, BattleEntity);
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);
            CollisionsComponent = BattleEntity.GetComponent<BattleTankCollisionsComponent>();

            if (BattleParams.BattleMode != BattleMode.DM)
            {
                if (BattleParams.BattleMode == BattleMode.CTF)
                {
                    RedPedestalEntity = PedestalTemplate.CreateEntity(CurrentMapInfo.Flags.Red.Position, team:RedTeamEntity, battle:BattleEntity);
                    BluePedestalEntity = PedestalTemplate.CreateEntity(CurrentMapInfo.Flags.Blue.Position, team:BlueTeamEntity, battle:BattleEntity);
                    RedFlagEntity = FlagTemplate.CreateEntity(CurrentMapInfo.Flags.Red.Position, team: RedTeamEntity, battle: BattleEntity);
                    BlueFlagEntity = FlagTemplate.CreateEntity(CurrentMapInfo.Flags.Blue.Position, team: BlueTeamEntity, battle: BattleEntity);
                }
                
                foreach (BattleLobbyPlayer battleLobbyPlayer in AllBattlePlayers)
                {
                    if (battleLobbyPlayer.Team.GetComponent<TeamColorComponent>().TeamColor == TeamColor.RED)
                        battleLobbyPlayer.Team = RedTeamEntity;
                    else
                        battleLobbyPlayer.Team = BlueTeamEntity;
                }
            }
        }
        
        public void AddPlayer(Player player)
        {
            // prepare client
            player.User.AddComponent(new UserEquipmentComponent(player.CurrentPreset.Weapon.EntityId, player.CurrentPreset.Hull.EntityId));
            player.ShareEntities(BattleLobbyEntity, BattleLobbyChatEntity);
            player.User.AddComponent(new BattleLobbyGroupComponent(BattleLobbyEntity));

            if (IsMatchMaking)
                player.User.AddComponent(new MatchMakingUserComponent());

            player.ShareEntities(AllBattlePlayers.Select(x => x.User));

            Entity teamEntity = null;
            List<BattleLobbyPlayer> teamPlayerList;

            if (BattleParams.BattleMode == BattleMode.DM)
            {
                teamPlayerList = DMTeamPlayers;
            }
            else if (RedTeamPlayers.Count < BlueTeamPlayers.Count)
            {
                teamEntity = RedTeamEntity;
                teamPlayerList = RedTeamPlayers;
            }
            else
            {
                teamEntity = BlueTeamEntity;
                teamPlayerList = BlueTeamPlayers;
            }

            player.User.AddComponent(teamEntity.GetComponent<TeamColorComponent>() ?? new TeamColorComponent(TeamColor.NONE));

            BattleLobbyPlayer battlePlayer = new BattleLobbyPlayer(player, teamEntity);
            player.BattleLobbyPlayer = battlePlayer;

            // broadcast client to other players
            AllBattlePlayers.Select(x => x.Player).ShareEntity(battlePlayer.User);

            lock (this)
                teamPlayerList.Add(battlePlayer);

            if (IsMatchMaking && BattleState == BattleState.WarmingUp || IsMatchMaking && BattleState == BattleState.Running)
            {
                player.SendEvent(new MatchMakingLobbyStartTimeEvent(new TimeSpan(0, 0, 10)), player.User);
                WaitingToJoinPlayers.Add(battlePlayer);
            }
        }

        private void RemovePlayer(BattleLobbyPlayer battlePlayer)
        {
            if (!DMTeamPlayers.Remove(battlePlayer)
                && !RedTeamPlayers.Remove(battlePlayer))
                BlueTeamPlayers.Remove(battlePlayer);
            WaitingToJoinPlayers.Remove(battlePlayer);
            
            // transfers owner ship to a random player in the lobby
            if (battlePlayer.Player == Owner)
            {
                if (AllBattlePlayers.Any())
                {
                    var allBattlePlayers = AllBattlePlayers.ToList();
                    Owner = allBattlePlayers[new Random().Next(allBattlePlayers.Count)].Player;
                    BattleLobbyEntity.RemoveComponent<UserGroupComponent>();
                    BattleLobbyEntity.AddComponent(new UserGroupComponent(Owner.User));
                }
            }

            battlePlayer.Player.BattleLobbyPlayer = null;

            battlePlayer.User.RemoveComponent<UserEquipmentComponent>();
            battlePlayer.Player.UnshareEntity(BattleLobbyEntity);
            battlePlayer.User.RemoveComponent<BattleLobbyGroupComponent>();
            battlePlayer.User.RemoveComponent<TeamColorComponent>();
            battlePlayer.Player.UnshareEntity(BattleLobbyChatEntity);

            if (IsMatchMaking)
                battlePlayer.User.RemoveComponent<MatchMakingUserComponent>();

            if (battlePlayer.User.GetComponent<MatchMakingUserReadyComponent>() != null)
                battlePlayer.User.RemoveComponent<MatchMakingUserReadyComponent>();

            battlePlayer.Player.UnshareEntities(AllBattlePlayers.Select(x => x.User));
            AllBattlePlayers.Select(x => x.Player).UnshareEntity(battlePlayer.User);

            ServerConnection.BattlePool.RemoveAll(p => !AllBattlePlayers.Any() && !p.IsMatchMaking);
        }

        private void StartBattle()
        {
            foreach (BattleLobbyPlayer battleLobbyPlayer in AllBattlePlayers)
                InitBattlePlayer(battleLobbyPlayer);
        }

        public void InitBattlePlayer(BattleLobbyPlayer battlePlayer)
        {
            battlePlayer.BattlePlayer = new BattlePlayer(battlePlayer, BattleEntity);

            battlePlayer.Player.ShareEntities(BattleEntity, RoundEntity, GeneralBattleChatEntity);

            if (BattleParams.BattleMode != BattleMode.DM)
            {
                battlePlayer.Player.ShareEntities(RedTeamEntity, BlueTeamEntity, TeamBattleChatEntity);

                if (BattleParams.BattleMode == BattleMode.CTF)
                {
                    battlePlayer.Player.ShareEntities(RedPedestalEntity, BluePedestalEntity);
                    if (!IsMatchMaking || BattleState == BattleState.Running)
                    {
                        battlePlayer.Player.ShareEntities(RedFlagEntity, BlueFlagEntity);
                    }
                }
            }

            foreach (BattleLobbyPlayer inBattlePlayer in MatchPlayers)
                battlePlayer.Player.ShareEntities(inBattlePlayer.BattlePlayer.GetEntities());

            MatchPlayers.Add(battlePlayer);

            MatchPlayers.Select(x => x.Player).ShareEntities(battlePlayer.BattlePlayer.GetEntities());
        }

        private void RemoveBattlePlayer(BattleLobbyPlayer battlePlayer)
        {
            battlePlayer.Player.UnshareEntities(BattleEntity, RoundEntity, GeneralBattleChatEntity);

            if (BattleParams.BattleMode != BattleMode.DM)
            {
                battlePlayer.Player.UnshareEntities(BlueTeamEntity, RedTeamEntity, TeamBattleChatEntity);

                if (BattleParams.BattleMode == BattleMode.CTF)
                {
                    Entity[] flags = { BlueFlagEntity, RedFlagEntity };
                    foreach (Entity flag in flags)
                    {
                        if (flag.GetComponent<TankGroupComponent>() != null && flag.GetComponent<FlagGroundedStateComponent>() == null)
                        {
                            if (flag.GetComponent<TankGroupComponent>().Key == battlePlayer.BattlePlayer.Tank.GetComponent<TankGroupComponent>().Key)
                            {
                                flag.AddComponent(new FlagGroundedStateComponent());
                                // TODO: drop flag at latest tank position
                                flag.ChangeComponent(new FlagPositionComponent(new Vector3(x: 0, y: 3, z: 0)));

                                MatchPlayers.Select(x => x.Player).SendEvent(new FlagDropEvent(IsUserAction: false), flag);
                            }
                        }
                    }
                }
            }

            MatchPlayers.Select(x => x.Player).UnshareEntities(battlePlayer.BattlePlayer.GetEntities());

            MatchPlayers.Remove(battlePlayer);

            if (BattleParams.BattleMode == BattleMode.CTF)
            {
                battlePlayer.Player.UnshareEntities(RedPedestalEntity, BluePedestalEntity);
                if (BattleState != BattleState.WarmingUp)
                {
                    battlePlayer.Player.UnshareEntities(RedFlagEntity, BlueFlagEntity);
                }
            }

            foreach (BattleLobbyPlayer inBattlePlayer in MatchPlayers)
                battlePlayer.Player.UnshareEntities(inBattlePlayer.BattlePlayer.GetEntities());

            if (IsMatchMaking) RemovePlayer(battlePlayer);
            battlePlayer.Reset();
        }

        private void ProcessExitedPlayers()
        {
            for (int i = 0; i < RedTeamPlayers.Count + BlueTeamPlayers.Count + DMTeamPlayers.Count; i++)
            {
                BattleLobbyPlayer battleLobbyPlayer;
                if (BattleParams.BattleMode == BattleMode.DM)
                {
                    battleLobbyPlayer = DMTeamPlayers[i];
                }
                else
                {
                    if (i < RedTeamPlayers.Count)
                        battleLobbyPlayer = RedTeamPlayers[i];
                    else
                        battleLobbyPlayer = BlueTeamPlayers[i - RedTeamPlayers.Count];
                }

                if (!battleLobbyPlayer.Player.IsActive || battleLobbyPlayer.WaitingForExit)
                {
                    if (battleLobbyPlayer.BattlePlayer != null)
                        RemoveBattlePlayer(battleLobbyPlayer);
                    else
                        RemovePlayer(battleLobbyPlayer);
                    i--;
                }
            }
        }

        private void ProcessBattleState(double deltaTime)
        {
            CountdownTimer -= deltaTime;
            switch (BattleState)
            {
                case BattleState.NotEnoughPlayers:
                    // Matchmaking only

                    if (IsMatchMaking && AllBattlePlayers.Any() && (RedTeamPlayers.Count == BlueTeamPlayers.Count || DMTeamPlayers.Count >= 2))
                    {
                        BattleLobbyEntity.AddComponent(new MatchMakingLobbyStartTimeComponent(new TimeSpan(0, 0, 10)));
                        CountdownTimer = 10;
                        BattleState = BattleState.StartCountdown;
                    }
                    break;
                case BattleState.StartCountdown:
                    // Matchmaking only

                    if (RedTeamPlayers.Count != BlueTeamPlayers.Count || DMTeamPlayers.Count >= 2)
                    {
                        Thread.Sleep(1000); // TODO: find a better solution for this (client crash when no delay)
                        BattleLobbyEntity.RemoveComponent<MatchMakingLobbyStartTimeComponent>();
                        BattleState = BattleState.NotEnoughPlayers;
                    }

                    if (CountdownTimer < 0)
                    {
                        BattleLobbyEntity.RemoveComponent<MatchMakingLobbyStartTimeComponent>();
                        BattleLobbyEntity.AddComponent(new MatchMakingLobbyStartingComponent());
                        CountdownTimer = 3;
                        BattleState = BattleState.Starting;
                    }

                    break;
                case BattleState.Starting:
                    if (IsMatchMaking)
                    {
                        if (RedTeamPlayers.Count != BlueTeamPlayers.Count || DMTeamPlayers.Count >= 2)
                        {
                            BattleLobbyEntity.RemoveComponent<MatchMakingLobbyStartingComponent>();
                            BattleState = BattleState.NotEnoughPlayers;
                            break;
                        }

                        if (CountdownTimer < 0)
                        {
                            BattleLobbyEntity.RemoveComponent<MatchMakingLobbyStartingComponent>();
                            RoundEntity.AddComponent(new RoundWarmingUpStateComponent());
                            BattleEntity.ChangeComponent(new BattleStartTimeComponent(new DateTimeOffset(DateTime.Now.AddSeconds(WarmUpSeconds))));
                            StartBattle();
                            CountdownTimer = WarmUpSeconds;
                            BattleState = BattleState.WarmingUp;
                            WarmUpState = WarmUpState.WarmingUp;
                        }
                    }
                    else
                    {
                        // todo replace matchmaking component with custom battle one
                        if (!AllBattlePlayers.Any())
                        {
                            BattleLobbyEntity.RemoveComponent<MatchMakingLobbyStartingComponent>();
                            BattleState = BattleState.CustomNotStarted;
                            break;
                        }

                        if (CountdownTimer < 0)
                        {
                            BattleLobbyEntity.RemoveComponent<MatchMakingLobbyStartingComponent>();
                            StartBattle();
                            BattleState = BattleState.Running;
                        }
                    }
                    break;
                case BattleState.WarmingUp:
                    // Matchmaking only
                    switch (WarmUpState)
                    {
                        case WarmUpState.WarmingUp:
                            {
                                if (CountdownTimer <= 4)
                                {
                                    foreach (BattleLobbyPlayer battleLobbyPlayer in MatchPlayers)
                                    {
                                        battleLobbyPlayer.BattlePlayer.Tank.RemoveComponent<TankMovableComponent>();
                                        battleLobbyPlayer.BattlePlayer.Weapon.RemoveComponent<ShootableComponent>();
                                    }
                                    CommandManager.BroadcastCommands(MatchPlayers.Select(x => x.Player),
                                        new EntityShareCommand(RedFlagEntity),
                                        new EntityShareCommand(BlueFlagEntity));
                                    WarmUpState = WarmUpState.MatchBegins;
                                }
                                break;
                            }
                        case WarmUpState.MatchBegins:
                            if (CountdownTimer <= 0)
                            {
                                foreach (BattleLobbyPlayer battleLobbyPlayer in MatchPlayers)
                                {
                                    battleLobbyPlayer.BattlePlayer.Tank.RemoveComponent<TankVisibleStateComponent>();
                                    battleLobbyPlayer.BattlePlayer.Tank.RemoveComponent<TankActiveStateComponent>();
                                    battleLobbyPlayer.BattlePlayer.TankState = TankState.New;
                                    battleLobbyPlayer.BattlePlayer.Tank.RemoveComponent<TankMovementComponent>();
                                    battleLobbyPlayer.BattlePlayer.Incarnation.RemoveComponent<TankIncarnationComponent>();
                                    battleLobbyPlayer.BattlePlayer.TankState = TankState.Spawn;
                                }
                                WarmUpState = WarmUpState.Respawning;
                                CountdownTimer = 1;
                            }
                            break;
                        case WarmUpState.Respawning:
                            if (CountdownTimer <= 0)
                            {
                                foreach (BattleLobbyPlayer battleLobbyPlayer in MatchPlayers)
                                    battleLobbyPlayer.BattlePlayer.Weapon.AddComponent(new ShootableComponent());

                                RoundEntity.RemoveComponent(typeof(RoundWarmingUpStateComponent));
                                BattleEntity.ChangeComponent(new BattleStartTimeComponent(new DateTimeOffset(DateTime.Now)));
                                BattleState = BattleState.Running;
                                CountdownTimer = 60*BattleParams.TimeLimit;
                            }
                            break;
                    }
                    break;
                case BattleState.Running:
                    if (IsMatchMaking)
                    {
                        if (!AllBattlePlayers.Any())
                            BattleState = BattleState.NotEnoughPlayers;
                    }
                    else if (!MatchPlayers.Any())
                    {
                        BattleState = BattleState.CustomNotStarted;
                    }

                    break;
            }
        }

        private void ProcessWaitingPlayers(double deltaTime)
        {
            for (int i = 0; i < WaitingToJoinPlayers.Count; i++)
            {
                BattleLobbyPlayer battleLobbyPlayer = WaitingToJoinPlayers[i];

                battleLobbyPlayer.MatchMakingJoinCountdown -= deltaTime;
                if (battleLobbyPlayer.MatchMakingJoinCountdown < 0)
                {
                    InitBattlePlayer(battleLobbyPlayer);

                    // Prevent joining and immediate exiting
                    battleLobbyPlayer.WaitingForExit = false;

                    WaitingToJoinPlayers.RemoveAt(i);
                    i--;
                }
            }
        }

        private void ProcessMatchPlayers(double deltaTime)
        {
            foreach (BattlePlayer battlePlayer in MatchPlayers.Select(x => x.BattlePlayer))
            {
                if (battlePlayer.TankState != TankState.Active && battlePlayer.TankState != TankState.New)
                {
                    battlePlayer.TankStateChangeCountdown -= deltaTime;
                }

                // switch state after it's ended
                if (battlePlayer.TankStateChangeCountdown < 0)
                {
                    switch (battlePlayer.TankState)
                    {
                        case TankState.Spawn:
                            battlePlayer.TankState = TankState.SemiActive;
                            battlePlayer.Tank.AddComponent(new TankVisibleStateComponent());
                            battlePlayer.Tank.AddComponent(new TankMovableComponent());
                            break;
                        case TankState.SemiActive:
                            if (!battlePlayer.WaitingForTankActivation)
                            {
                                battlePlayer.Tank.AddComponent(new TankStateTimeOutComponent());
                                battlePlayer.WaitingForTankActivation = true;
                            }
                            break;
                        case TankState.Dead:
                            battlePlayer.TankState = TankState.Spawn;
                            battlePlayer.Tank.RemoveComponent<TankVisibleStateComponent>();
                            battlePlayer.Tank.RemoveComponent<TankMovableComponent>();
                            break;
                    }
                }

                if (battlePlayer.CollisionsPhase == CollisionsComponent.SemiActiveCollisionsPhase)
                {
                    CollisionsComponent.SemiActiveCollisionsPhase++;

                    battlePlayer.Tank.RemoveComponent<TankStateTimeOutComponent>();
                    BattleEntity.ChangeComponent(CollisionsComponent);

                    battlePlayer.TankState = TankState.Active;
                    battlePlayer.WaitingForTankActivation = false;
                }

                foreach (KeyValuePair<Type, TranslatedEvent> pair in battlePlayer.TranslatedEvents)
                {
                    (from matchPlayer in MatchPlayers
                     where matchPlayer.BattlePlayer != battlePlayer
                     select matchPlayer.Player).SendEvent(pair.Value.Event, pair.Value.TankPart);
                    battlePlayer.TranslatedEvents.TryRemove(pair.Key, out _);
                }
            }
        }

        private void ProcessDroppedFlags()
        {
            DateTime currentTime = DateTime.Now;
            foreach (KeyValuePair<Entity, DateTime> entry in DroppedFlags.ToList())
            {
                if (DateTime.Compare(entry.Value, currentTime) <= 0 || BattleState != BattleState.Running)
                {
                    DroppedFlags.Remove(entry.Key);
                    entry.Key.RemoveComponent<TankGroupComponent>();
                    entry.Key.RemoveComponent<FlagGroundedStateComponent>();

                    Entity newFlag;
                    if (RedFlagEntity.GetComponent<TeamGroupComponent>().Key == entry.Key.GetComponent<TeamGroupComponent>().Key)
                    {
                        RedFlagEntity = FlagTemplate.CreateEntity(CurrentMapInfo.Flags.Red.Position, team: RedTeamEntity, battle: BattleEntity);
                        newFlag = RedFlagEntity;
                    }
                    else
                    {
                        BlueFlagEntity = FlagTemplate.CreateEntity(CurrentMapInfo.Flags.Blue.Position, team: BlueTeamEntity, battle: BattleEntity);
                        newFlag = BlueFlagEntity;
                    }

                    foreach (Player player in MatchPlayers.Select(x => x.Player))
                    {
                        player.SendEvent(new FlagReturnEvent(), entry.Key);
                        player.UnshareEntity(entry.Key);
                        player.ShareEntity(newFlag);
                    }
                }
            }
        }
        
        public void Tick(double deltaTime)
        {
            lock (this)
            {
                ProcessExitedPlayers();
                ProcessBattleState(deltaTime);
                ProcessWaitingPlayers(deltaTime);
                ProcessMatchPlayers(deltaTime);
                ProcessDroppedFlags();
            }
        }

        public void UpdatedScore(Player player)
        {
            int? neededDifference = null;
            if (BattleParams.BattleMode == BattleMode.CTF)
            {
                neededDifference = 6;
            } else
            {
                if (BattleParams.BattleMode == BattleMode.TDM)
                {
                    neededDifference = 30;
                }
            }

            if (neededDifference != null)
            {
                if (Math.Abs(BlueTeamEntity.GetComponent<TeamScoreComponent>().Score - RedTeamEntity.GetComponent<TeamScoreComponent>().Score) >= neededDifference)
                {

                    if (BattleEntity.GetComponent<RoundDisbalancedComponent>() == null)
                    {
                        Entity[] teams = { BlueTeamEntity, RedTeamEntity };
                        Entity loserTeam = teams.Single(t => t.GetComponent<TeamColorComponent>().TeamColor != player.BattleLobbyPlayer.Team.GetComponent<TeamColorComponent>().TeamColor);
                        TeamColor loserColor = loserTeam.GetComponent<TeamColorComponent>().TeamColor;

                        Component roundDisbalancedComponent = new RoundDisbalancedComponent(Loser: loserColor, InitialDominationTimerSec: 30, FinishTime: new TXDate(new TimeSpan(0, 0, 30)));

                        BattleEntity.AddComponent(roundDisbalancedComponent);
                    }
                } 

                else if (BattleEntity.GetComponent<RoundDisbalancedComponent>() != null)
                {
                    //TODO: restore round balance
                }
            }
        }
        
        public void UpdateUserStatistics(Player player, int xp, int kills, int killAssists, int death)
        {
            // TODO: rank up effect/system
            RoundUserStatisticsComponent roundUserStatisticsComponent = player.BattleLobbyPlayer.BattlePlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>();
            UserExperienceComponent userExperienceComponent = player.User.GetComponent<UserExperienceComponent>();

            roundUserStatisticsComponent.ScoreWithoutBonuses += xp;
            roundUserStatisticsComponent.Kills += kills;
            roundUserStatisticsComponent.KillAssists += killAssists;
            roundUserStatisticsComponent.Deaths += death;

            userExperienceComponent.Experience += xp;

            player.BattleLobbyPlayer.BattlePlayer.RoundUser.ChangeComponent(roundUserStatisticsComponent);
            player.User.ChangeComponent(userExperienceComponent);

            MatchPlayers.Select(x => x.Player).SendEvent(new RoundUserStatisticsUpdatedEvent(), player.BattleLobbyPlayer.BattlePlayer.RoundUser);
        }
        
        private static readonly Dictionary<BattleMode, Type> BattleEntityCreators = new Dictionary<BattleMode, Type>
        {
            { BattleMode.DM, typeof(DMTemplate) },
            { BattleMode.TDM, typeof(TDMTemplate) },
            { BattleMode.CTF, typeof(CTFTemplate) },
        };

        private static readonly Dictionary<GravityType, float> GravityTypes = new Dictionary<GravityType, float>
        {
            { GravityType.EARTH, 9.81f },
            { GravityType.SUPER_EARTH, 30 },
            { GravityType.MOON, 1.62f },
            { GravityType.MARS, 3.71f }
        };

        private static readonly List<Entity> MatchMakingMaps = new List<Entity>
        {
            Maps.GlobalItems.Silence,
            Maps.GlobalItems.Nightiran,
            Maps.GlobalItems.Acidlake, 
            Maps.GlobalItems.Sandbox,
            Maps.GlobalItems.Iran,
            Maps.GlobalItems.Area159,
            Maps.GlobalItems.Repin,
            Maps.GlobalItems.Westprime,
            Maps.GlobalItems.Boombox, 
            Maps.GlobalItems.Silencemoon,
            Maps.GlobalItems.Rio,
            Maps.GlobalItems.MassacremarsBG,
            Maps.GlobalItems.Massacre,
            Maps.GlobalItems.Kungur
        };

        public ClientBattleParams BattleParams { get; set; }
        public int WarmUpSeconds { get; set; }
        public Entity MapEntity { get; private set; }
        public bool IsMatchMaking { get; }
        public bool IsOpen { get; set; }

        public MapInfo CurrentMapInfo { get; set; }
        public IList<SpawnPoint> DeathmatchSpawnPoints { get; set; }
        public TeamSpawnPointList TeamSpawnPoints { get; set; }

        public BattleState BattleState { get; set; }
        public WarmUpState WarmUpState { get; set; }
        public double CountdownTimer { get; set; }

        // All players (not only in match)
        public List<BattleLobbyPlayer> RedTeamPlayers { get; } = new List<BattleLobbyPlayer>();
        public List<BattleLobbyPlayer> BlueTeamPlayers { get; } = new List<BattleLobbyPlayer>();
        public List<BattleLobbyPlayer> DMTeamPlayers { get; } = new List<BattleLobbyPlayer>();
        public IEnumerable<BattleLobbyPlayer> AllBattlePlayers => RedTeamPlayers.Concat(BlueTeamPlayers).Concat(DMTeamPlayers);

        public List<BattleLobbyPlayer> MatchPlayers { get; } = new List<BattleLobbyPlayer>();
        public List<BattleLobbyPlayer> WaitingToJoinPlayers { get; } = new List<BattleLobbyPlayer>();

        public Entity BattleEntity { get; set; }
        public Entity BattleLobbyEntity { get; set; }
        public Entity RoundEntity { get; set; }
        public BattleTankCollisionsComponent CollisionsComponent { get; set; }

        public Entity GeneralBattleChatEntity { get; set; }
        public Entity TeamBattleChatEntity { get; set; }
        public Entity BattleLobbyChatEntity { get; set; }

        public Entity RedTeamEntity { get; set; }
        public Entity BlueTeamEntity { get; set; }
        public Entity RedPedestalEntity { get; set; }
        public Entity BluePedestalEntity { get; set; }
        public Entity RedFlagEntity { get; set; }
        public Entity BlueFlagEntity { get; set; }
        public Dictionary<Entity, DateTime> DroppedFlags { get; } = new Dictionary<Entity, DateTime> { };
        public long? FlagBlockedTankKey { get; set; }
        public Player Owner { get; set; }
    }
}
