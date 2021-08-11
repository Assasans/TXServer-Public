using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Battles.Matchmaking;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Time;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events.Battle.Score;
using TXServer.ECSSystem.Events.Matchmaking;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public class MatchMakingBattleHandler : IBattleTypeHandler
        {
            public Battle Battle { get; init; }

            private readonly List<BattleTankPlayer> WaitingToJoinPlayers = new();

            public WarmUpState WarmUpState;

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
                    Battle.Params = new ClientBattleParams(MatchMaking.BattleModePicker(), MapId: matchMakingMaps[index].MapId,
                        MaxPlayers: 20, TimeLimit: 10, ScoreLimit: 100, FriendlyFire: false, GravityType.EARTH, KillZoneEnabled: true, DisabledModules: false);
                }

                (Battle.MapEntity, Battle.Params.MaxPlayers) = Battle.ConvertMapParams(Battle.Params, Battle.IsMatchMaking);
                Battle.WarmUpSeconds = 60;

                Battle.BattleLobbyEntity = MatchMakingLobbyTemplate.CreateEntity(Battle.Params, Battle.MapEntity, Battle.GravityTypes[Battle.Params.Gravity]);
            }

            private int GetWarmUpTime()
            {
                int lowLeagues = 0;
                int highLeagues = 0;

                foreach (BattleTankPlayer player in Battle.JoinedTankPlayers)
                {
                    if (new[] {Leagues.GlobalItems.Training, Leagues.GlobalItems.Bronze}
                        .Contains(player.Player.Data.League)) lowLeagues++;
                    else highLeagues++;
                }

                int warmUpTime = highLeagues >= lowLeagues ? 90 : 60;
                Battle.BattleEntity.ChangeComponent<TimeLimitComponent>(component =>
                    component.WarmingUpTimeLimitSet = warmUpTime);
                return warmUpTime;
            }

            public void Tick()
            {
                WaitingToJoinPlayers.RemoveAll(battlePlayer =>
                {
                    if (DateTime.UtcNow < battlePlayer.MatchMakingJoinCountdown) return false;
                    if (Battle.ForcePause)
                    {
                        battlePlayer.MatchMakingJoinCountdown = DateTime.UtcNow.AddSeconds(10);
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
                            Battle.WarmUpSeconds = GetWarmUpTime();
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
                                    foreach (BattleTankPlayer battlePlayer in Battle.MatchTankPlayers)
                                    {
                                        battlePlayer.MatchPlayer.KeepDisabled = true;
                                        battlePlayer.MatchPlayer.DisableTank(true);
                                    }

                                    Battle.CompleteWarmUp();
                                    WarmUpState = WarmUpState.MatchBegins;
                                }
                                break;
                            case WarmUpState.MatchBegins:
                                if (Battle.CountdownTimer <= 0)
                                {
                                    foreach (BattleTankPlayer battlePlayer in Battle.MatchTankPlayers)
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
                        var roundDisbalancedComponent = new RoundDisbalancedComponent(Loser: Battle.LosingTeam, InitialDominationTimerSec: 30, FinishTime: DateTime.UtcNow.AddSeconds(30));

                        Battle.BattleEntity.AddComponent(roundDisbalancedComponent);
                        Battle.BattleEntity.AddComponent(new RoundComponent());

                        StopTimeBeforeDomination = Battle.RoundEntity.GetComponent<RoundStopTimeComponent>();
                        Battle.RoundEntity.ChangeComponent(new RoundStopTimeComponent(DateTimeOffset.UtcNow.AddSeconds(30)));

                        DominationStartTime = DateTime.UtcNow.AddSeconds(30);
                    }
                    else
                    {
                        Battle.RoundEntity.ChangeComponent(StopTimeBeforeDomination);
                        StopTimeBeforeDomination = null;

                        Battle.PlayersInMap.SendEvent(new RoundBalanceRestoredEvent(), Battle.BattleEntity);
                        Battle.BattleEntity.RemoveComponent<RoundComponent>();
                        Battle.BattleEntity.RemoveComponent<RoundDisbalancedComponent>();
                    }

                    LastLosingTeam = Battle.LosingTeam;
                }

                if (LastLosingTeam != TeamColor.NONE &&
                    DateTime.UtcNow > DominationStartTime &&
                    Battle.BattleState != BattleState.Ended)
                    Battle.FinishBattle();
            }

            public void OnPlayerAdded(BattleTankPlayer battlePlayer)
            {
                battlePlayer.User.AddComponent(new MatchMakingUserComponent());
                if (Battle.BattleState is BattleState.WarmUp or BattleState.Running && !Battle.ForcePause)
                {
                    battlePlayer.SendEvent(new MatchMakingLobbyStartTimeEvent { StartTime = DateTime.UtcNow.AddSeconds(10) }, battlePlayer.User);
                    WaitingToJoinPlayers.Add(battlePlayer);
                }
            }

            public void OnPlayerRemoved(BattleTankPlayer battlePlayer)
            {
                WaitingToJoinPlayers.Remove(battlePlayer);
                battlePlayer.User.RemoveComponent<MatchMakingUserComponent>();
            }
        }
    }
}
