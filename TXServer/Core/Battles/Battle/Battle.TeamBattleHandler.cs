using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle.Score;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public abstract class TeamBattleHandler : IBattleModeHandler
        {
            public Battle Battle { get; init; }

            public Entity RedTeamEntity { get; private set; }
            public Entity BlueTeamEntity { get; private set; }

            public Entity TeamBattleChatEntity { get; private set; }
            public TeamSpawnPointList SpawnPoints { get; private set; }

            public List<BattlePlayer> RedTeamPlayers { get; private set; } = new List<BattlePlayer>();
            public List<BattlePlayer> BlueTeamPlayers { get; private set; } = new List<BattlePlayer>();
            public IEnumerable<BattlePlayer> Players => RedTeamPlayers.Concat(BlueTeamPlayers);

            public bool IsEnoughPlayers => Players.Any() && RedTeamPlayers.Count == BlueTeamPlayers.Count;
            public abstract TeamColor LosingTeam { get; }

            private static IEnumerable<UserResult> GetTeamResults(List<BattlePlayer> players) => players.Where(x => x.MatchPlayer != null).Select(x => x.MatchPlayer.UserResult);
            public IEnumerable<UserResult> RedTeamResults => GetTeamResults(RedTeamPlayers);
            public IEnumerable<UserResult> BlueTeamResults => GetTeamResults(BlueTeamPlayers);

            public virtual BattleView BattleViewFor(BattlePlayer battlePlayer)
            {
                bool isRed = battlePlayer.Team == RedTeamEntity;
                return new BattleView
                {
                    AllyTeamColor = isRed ? TeamColor.RED : TeamColor.BLUE,
                    EnemyTeamColor = isRed ? TeamColor.BLUE : TeamColor.RED,

                    AllyTeamEntity = isRed ? RedTeamEntity : BlueTeamEntity,
                    EnemyTeamEntity = isRed ? BlueTeamEntity : RedTeamEntity,

                    AllyTeamPlayers = isRed ? RedTeamPlayers : BlueTeamPlayers,
                    EnemyTeamPlayers = isRed ? BlueTeamPlayers : RedTeamPlayers,

                    AllyTeamResults = isRed ? RedTeamResults : BlueTeamResults,
                    EnemyTeamResults = isRed ? BlueTeamResults : RedTeamResults,

                    SpawnPoints = isRed ? SpawnPoints.RedTeam : SpawnPoints.BlueTeam
                };
            }
            public int EnemyCountFor(BattlePlayer battlePlayer) => BattleViewFor(battlePlayer).EnemyTeamPlayers.Count;
            public TeamBattleResult TeamBattleResultFor(BattlePlayer battlePlayer) => (RedTeamEntity.GetComponent<TeamScoreComponent>().Score - BlueTeamEntity.GetComponent<TeamScoreComponent>().Score) switch
            {
                > 0 => battlePlayer.Team == RedTeamEntity ? TeamBattleResult.WIN : TeamBattleResult.DEFEAT,
                < 0 => battlePlayer.Team == BlueTeamEntity ? TeamBattleResult.WIN : TeamBattleResult.DEFEAT,
                _ => TeamBattleResult.DRAW
            };

            public void SortRoundUsers()
            {
                foreach (List<BattlePlayer> list in new[] { RedTeamPlayers, BlueTeamPlayers })
                    ((IBattleModeHandler)this).SortRoundUsers(list);
            }

            public virtual void SetupBattle()
            {
                RedTeamEntity = TeamTemplate.CreateEntity(TeamColor.RED, Battle.BattleEntity);
                BlueTeamEntity = TeamTemplate.CreateEntity(TeamColor.BLUE, Battle.BattleEntity);
                TeamBattleChatEntity = TeamBattleChatTemplate.CreateEntity();

                var teamModesSpawnPoints = new Dictionary<BattleMode, TeamSpawnPointList>
                {
                    { BattleMode.CTF, Battle.CurrentMapInfo.SpawnPoints.CaptureTheFlag },
                    { BattleMode.TDM, Battle.CurrentMapInfo.SpawnPoints.TeamDeathmatch }
                };

                SpawnPoints = teamModesSpawnPoints[Battle.Params.BattleMode] ?? teamModesSpawnPoints.Values.Where(b => b != null).First();
            }

            public void SetupBattle(IBattleModeHandler prevHandler)
            {
                SetupBattle();

                if (prevHandler is TeamBattleHandler teamBattleHandler)
                {
                    RedTeamPlayers = teamBattleHandler.RedTeamPlayers;
                    BlueTeamPlayers = teamBattleHandler.BlueTeamPlayers;

                    foreach (BattlePlayer battlePlayer in Players)
                        battlePlayer.Team = battlePlayer.Team.GetComponent<TeamColorComponent>().TeamColor == TeamColor.RED ? RedTeamEntity : BlueTeamEntity;
                }
                else
                {
                    foreach (BattlePlayer battlePlayer in prevHandler.Players)
                    {
                        Entity teamEntity;
                        List<BattlePlayer> teamPlayerList;

                        if (RedTeamPlayers.Count < BlueTeamPlayers.Count)
                        {
                            teamEntity = RedTeamEntity;
                            teamPlayerList = RedTeamPlayers;
                        }
                        else
                        {
                            teamEntity = BlueTeamEntity;
                            teamPlayerList = BlueTeamPlayers;
                        }

                        battlePlayer.Team = teamEntity;
                        teamPlayerList.Add(battlePlayer);

                        battlePlayer.Player.User.ChangeComponent(teamEntity.GetComponent<TeamColorComponent>());
                    }
                }
            }

            public virtual void CompleteWarmUp()
            {
            }

            public virtual void OnFinish()
            {
            }

            public virtual void Tick()
            {
            }

            public BattlePlayer AddPlayer(Player player, bool isSpectator)
            {
                List<BattlePlayer> teamPlayerList = null;
                Entity teamEntity = null;
                
                if (!isSpectator)
                {
                    List<List<BattlePlayer>> teamPlayerLists = new() {RedTeamPlayers, BlueTeamPlayers};
                    Dictionary<List<BattlePlayer>, Entity> entityPlayersDict = new()
                        {{RedTeamPlayers, RedTeamEntity}, {BlueTeamPlayers, BlueTeamEntity}};
                    
                    if (Battle.IsMatchMaking)
                    {
                        foreach (var playersList in teamPlayerLists.Where(y => player.IsInSquad).Where(
                            playersList => player.SquadPlayer.Squad.Participants.Select(x => x.Player.BattlePlayer)
                                .Intersect(playersList).Any()))
                        {
                            teamEntity = entityPlayersDict[playersList];
                            teamPlayerList = playersList;
                            break;
                        }
                    }

                    if (teamEntity == null)
                    {
                        if (RedTeamPlayers.Count < BlueTeamPlayers.Count)
                        {
                            teamEntity = RedTeamEntity;
                            teamPlayerList = RedTeamPlayers;
                        }
                        else
                        {
                            teamEntity = BlueTeamEntity;
                            teamPlayerList = BlueTeamPlayers;
                        }
                    }

                    player.User.AddComponent(teamEntity.GetComponent<TeamColorComponent>());
                }

                BattlePlayer battlePlayer = new(Battle, player, teamEntity, isSpectator);
                player.BattlePlayer = battlePlayer;

                if (!isSpectator) teamPlayerList.Add(battlePlayer);

                return battlePlayer;
            }

            public void RemovePlayer(BattlePlayer battlePlayer)
            {
                if (!RedTeamPlayers.Remove(battlePlayer))
                    BlueTeamPlayers.Remove(battlePlayer);

                if (!battlePlayer.IsSpectator)
                    battlePlayer.User.RemoveComponent<TeamColorComponent>();
                battlePlayer.Player.BattlePlayer = null;
            }

            public virtual void OnMatchJoin(BattlePlayer battlePlayer)
            {
                battlePlayer.Player.ShareEntities(RedTeamEntity, BlueTeamEntity, TeamBattleChatEntity);
            }

            public virtual void OnMatchLeave(BattlePlayer battlePlayer)
            {
                battlePlayer.Player.UnshareEntities(BlueTeamEntity, RedTeamEntity, TeamBattleChatEntity);
            }
        }
    }
}
