﻿using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events.Battle.Score;
using TXServer.ECSSystem.Events.Matchmaking;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public class MatchMakingBattleHandler : IBattleTypeHandler
        {
            public Battle Battle { get; init; }

            private readonly List<BattlePlayer> WaitingToJoinPlayers = new();

            private WarmUpState WarmUpState;

            private TeamColor LastLosingTeam;
            private DateTime DominationStartTime;

            public void SetupBattle()
            {
                if (Battle.Params == null)
                {
                    List<MapInfo> matchMakingMaps = ServerConnection.ServerMapInfo.Values.Where(p => p.MatchMaking).ToList();
                    int index = new Random().Next(matchMakingMaps.Count);
                    Battle.Params = new ClientBattleParams(BattleMode: BattleMode.CTF, MapId: matchMakingMaps[index].MapId, MaxPlayers: 20, TimeLimit: 10,
                        ScoreLimit: 100, FriendlyFire: false, Gravity: GravityType.EARTH, KillZoneEnabled: true, DisabledModules: false);
                }

                (Battle.MapEntity, Battle.Params.MaxPlayers) = Battle.ConvertMapParams(Battle.Params, Battle.IsMatchMaking);
                Battle.WarmUpSeconds = 60; // TODO: 1min in Bronze league, 1,5min in Silver, Gold & Master leagues

                Battle.BattleLobbyEntity = MatchMakingLobbyTemplate.CreateEntity(Battle.Params, Battle.MapEntity, GravityTypes[Battle.Params.Gravity]);
            }

            public void Tick()
            {
                WaitingToJoinPlayers.RemoveAll(battlePlayer =>
                {
                    if (DateTime.Now > battlePlayer.MatchMakingJoinCountdown)
                    {
                        Battle.InitMatchPlayer(battlePlayer);

                        // Prevent joining and immediate exiting
                        battlePlayer.WaitingForExit = false;

                        return true;
                    }

                    return false;
                });

                switch (Battle.BattleState)
                {
                    case BattleState.NotEnoughPlayers:
                        if (Battle.IsEnoughPlayers)
                            Battle.BattleState = BattleState.StartCountdown;

                        break;
                    case BattleState.StartCountdown:
                        if (!Battle.IsEnoughPlayers)
                        {
                            //Thread.Sleep(1000); // TODO: find a better solution for this (client crash when no delay)
                            Battle.BattleState = BattleState.NotEnoughPlayers;
                        }

                        if (Battle.CountdownTimer < 0)
                            Battle.BattleState = BattleState.Starting;

                        break;
                    case BattleState.Starting:
                        if (!Battle.IsEnoughPlayers)
                        {
                            Battle.BattleState = BattleState.NotEnoughPlayers;
                            break;
                        }

                        if (Battle.CountdownTimer < 0)
                        {
                            Battle.StartBattle();
                            Battle.BattleState = BattleState.WarmUp;
                            WarmUpState = WarmUpState.WarmingUp;
                        }
                        break;
                    case BattleState.WarmUp:
                        switch (WarmUpState)
                        {
                            case WarmUpState.WarmingUp:
                                if (Battle.CountdownTimer <= 4)
                                {
                                    foreach (BattlePlayer battleLobbyPlayer in Battle.MatchPlayers)
                                    {
                                        battleLobbyPlayer.MatchPlayer.Tank.RemoveComponent<TankMovableComponent>();
                                        battleLobbyPlayer.MatchPlayer.Weapon.RemoveComponent<ShootableComponent>();
                                    }
                                    Battle.CompleteWarmUp();
                                    WarmUpState = WarmUpState.MatchBegins;
                                }
                                break;
                            case WarmUpState.MatchBegins:
                                if (Battle.CountdownTimer <= 0)
                                {
                                    foreach (BattlePlayer battleLobbyPlayer in Battle.MatchPlayers)
                                    {
                                        battleLobbyPlayer.MatchPlayer.Tank.RemoveComponent<TankVisibleStateComponent>();
                                        battleLobbyPlayer.MatchPlayer.Tank.RemoveComponent<TankActiveStateComponent>();
                                        battleLobbyPlayer.MatchPlayer.TankState = TankState.New;
                                        battleLobbyPlayer.MatchPlayer.Tank.RemoveComponent<TankMovementComponent>();
                                        battleLobbyPlayer.MatchPlayer.Incarnation.RemoveComponent<TankIncarnationComponent>();
                                        battleLobbyPlayer.MatchPlayer.TankState = TankState.Spawn;
                                    }
                                    WarmUpState = WarmUpState.Respawning;
                                    Battle.CountdownTimer = 1;
                                }
                                break;
                            case WarmUpState.Respawning:
                                if (Battle.CountdownTimer <= 0)
                                {
                                    foreach (BattlePlayer battleLobbyPlayer in Battle.MatchPlayers)
                                        battleLobbyPlayer.MatchPlayer.Weapon.AddComponent(new ShootableComponent());

                                    Battle.BattleState = BattleState.Running;
                                }
                                break;
                        }
                        break;
                    case BattleState.Running:
                        if (!Battle.AllBattlePlayers.Any())
                        {
                            Battle.StartBattle();
                            Battle.BattleState = BattleState.NotEnoughPlayers;
                        }

                        if (Battle.CountdownTimer < 0)
                            Battle.FinishBattle();
                        break;
                }

                bool timeAllowsCheck = Battle.CountdownTimer < Battle.Params.TimeLimit * 60 - 120 && Battle.CountdownTimer > 120;

                if (timeAllowsCheck && Battle.LosingTeam != LastLosingTeam)
                {
                    if (Battle.LosingTeam != TeamColor.NONE)
                    {
                        var roundDisbalancedComponent = new RoundDisbalancedComponent(Loser: Battle.LosingTeam, InitialDominationTimerSec: 30, FinishTime: new TXDate(new TimeSpan(0, 0, 30)));

                        Battle.BattleEntity.AddComponent(roundDisbalancedComponent);
                        Battle.BattleEntity.AddComponent(new RoundComponent());

                        DominationStartTime = DateTime.Now.AddSeconds(30);
                    }
                    else
                    {
                        Battle.MatchPlayers.Select(x => x.Player).SendEvent(new RoundBalanceRestoredEvent(), Battle.BattleEntity);
                        Battle.BattleEntity.RemoveComponent<RoundComponent>();
                        Battle.BattleEntity.RemoveComponent<RoundDisbalancedComponent>();
                    }

                    LastLosingTeam = Battle.LosingTeam;
                }

                if (LastLosingTeam != TeamColor.NONE &&
                    DateTime.Now > DominationStartTime &&
                    Battle.BattleState != BattleState.Ended)
                    Battle.FinishBattle();
            }

            public void OnPlayerAdded(BattlePlayer battlePlayer)
            {
                battlePlayer.User.AddComponent(new MatchMakingUserComponent());
                if (Battle.BattleState is BattleState.WarmUp or BattleState.Running)
                {
                    battlePlayer.Player.SendEvent(new MatchMakingLobbyStartTimeEvent(new TimeSpan(0, 0, 10)), battlePlayer.User);
                    WaitingToJoinPlayers.Add(battlePlayer);
                }
            }

            public void OnPlayerRemoved(BattlePlayer battlePlayer)
            {
                WaitingToJoinPlayers.Remove(battlePlayer);
                battlePlayer.User.RemoveComponent<MatchMakingUserComponent>();

                if (Battle.BattleState != BattleState.Ended && Battle.EnemyCountFor(battlePlayer) > 0)
                {
                    // TODO: add deserter status only when player leaves 2 out of 4 last battles prematurely & conditions above
                    BattleLeaveCounterComponent battleLeaveCounterComponent = battlePlayer.Player.User.GetComponent<BattleLeaveCounterComponent>();
                    battleLeaveCounterComponent.Value += 1;
                    battleLeaveCounterComponent.NeedGoodBattles += 2;
                    battlePlayer.Player.User.ChangeComponent(battleLeaveCounterComponent);
                }
            }
        }
    }
}
