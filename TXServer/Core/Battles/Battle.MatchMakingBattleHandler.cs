using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Time;
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
            private RoundStopTimeComponent StopTimeBeforeDomination;

            public void SetupBattle()
            {
                if (Battle.Params == null)
                {
                    List<MapInfo> matchMakingMaps = ServerConnection.ServerMapInfo.Values.Where(p => p.MatchMaking).ToList();
                    Random random = new();
                    int index = random.Next(matchMakingMaps.Count);
                    Battle.Params = new ClientBattleParams(BattleMode: BattleModePicker.MatchMakingBattleModePicker(random), MapId: matchMakingMaps[index].MapId,
                        MaxPlayers: 20, TimeLimit: 10, ScoreLimit: 100, FriendlyFire: false, Gravity: GravityType.EARTH, KillZoneEnabled: true, DisabledModules: false);
                }

                (Battle.MapEntity, Battle.Params.MaxPlayers) = Battle.ConvertMapParams(Battle.Params, Battle.IsMatchMaking);
                Battle.WarmUpSeconds = 60; // TODO: 1min in Bronze league, 1,5min in Silver, Gold & Master leagues

                Battle.BattleLobbyEntity = MatchMakingLobbyTemplate.CreateEntity(Battle.Params, Battle.MapEntity, Battle.GravityTypes[Battle.Params.Gravity]);
            }

            public void Tick()
            {
                WaitingToJoinPlayers.RemoveAll(battlePlayer =>
                {
                    if (DateTime.Now < battlePlayer.MatchMakingJoinCountdown) return false;
                    if (Battle.ForcePause)
                    {
                        battlePlayer.MatchMakingJoinCountdown = DateTime.Now.AddSeconds(10);
                        return false;
                    }
                    
                    Battle.InitMatchPlayer(battlePlayer);

                    // Prevent joining and immediate exiting
                    battlePlayer.WaitingForExit = false;

                    return true;
                });

                switch (Battle.BattleState)
                {
                    case BattleState.NotEnoughPlayers:
                        if (Battle.IsEnoughPlayers && !Battle.ForcePause || Battle.ForceStart)
                            Battle.BattleState = BattleState.StartCountdown;

                        break;
                    case BattleState.StartCountdown:
                        if (!Battle.IsEnoughPlayers && !Battle.ForceStart || Battle.ForcePause)
                            Battle.BattleState = BattleState.NotEnoughPlayers;

                        if (Battle.CountdownTimer < 0)
                            Battle.BattleState = BattleState.Starting;

                        break;
                    case BattleState.Starting:
                        if (!Battle.IsEnoughPlayers && !Battle.ForceStart || Battle.ForcePause)
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
                                    foreach (BattlePlayer battlePlayer in Battle.MatchPlayers.Where(x => !x.IsSpectator))
                                    {
                                        battlePlayer.MatchPlayer.KeepDisabled = true;
                                        battlePlayer.MatchPlayer.DisableTank();
                                    }

                                    Battle.CompleteWarmUp();
                                    WarmUpState = WarmUpState.MatchBegins;
                                }
                                break;
                            case WarmUpState.MatchBegins:
                                if (Battle.CountdownTimer <= 0)
                                {
                                    foreach (BattlePlayer battlePlayer in Battle.MatchPlayers)
                                    {
                                        battlePlayer.MatchPlayer.KeepDisabled = false;
                                        battlePlayer.MatchPlayer.TankState = TankState.Spawn;
                                    }

                                    WarmUpState = WarmUpState.Respawning;
                                    Battle.CountdownTimer = 1;
                                }
                                break;
                            case WarmUpState.Respawning:
                                if (Battle.CountdownTimer <= 0)
                                {
                                    Battle.BattleState = BattleState.Running;
                                }
                                break;
                        }
                        break;
                    case BattleState.Running:
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

                        StopTimeBeforeDomination = Battle.RoundEntity.GetComponent<RoundStopTimeComponent>();
                        Battle.RoundEntity.ChangeComponent(new RoundStopTimeComponent(DateTimeOffset.Now.AddSeconds(30)));

                        DominationStartTime = DateTime.Now.AddSeconds(30);
                    }
                    else
                    {
                        Battle.RoundEntity.ChangeComponent(StopTimeBeforeDomination);
                        StopTimeBeforeDomination = null;

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
                if (Battle.BattleState is BattleState.WarmUp or BattleState.Running && !Battle.ForcePause)
                {
                    battlePlayer.Player.SendEvent(new MatchMakingLobbyStartTimeEvent(new TimeSpan(0, 0, 10)), battlePlayer.User);
                    WaitingToJoinPlayers.Add(battlePlayer);
                }
            }

            public void OnPlayerRemoved(BattlePlayer battlePlayer)
            {
                WaitingToJoinPlayers.Remove(battlePlayer);
                battlePlayer.User.RemoveComponent<MatchMakingUserComponent>();

                if (battlePlayer.Player.IsInMatch && Battle.EnemyCountFor(battlePlayer) > 0)
                {
                    // TODO: add deserter status only when player leaves 2 out of 4 last battles prematurely & conditions above
                    battlePlayer.Player.User.ChangeComponent<BattleLeaveCounterComponent>(component =>
                    {
                        component.Value += 1;
                        component.NeedGoodBattles += 2;
                    });
                }
            }
        }
    }
}
