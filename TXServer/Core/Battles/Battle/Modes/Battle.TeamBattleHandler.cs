using System.Collections.Generic;
using System.Linq;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.EntityTemplates.Battle;
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

            public List<BattleTankPlayer> RedTeamPlayers { get; private set; } = new();
            public List<BattleTankPlayer> BlueTeamPlayers { get; private set; } = new();
            public IEnumerable<BattleTankPlayer> Players => RedTeamPlayers.Concat(BlueTeamPlayers);

            public bool IsEnoughPlayers => Players.Any() && RedTeamPlayers.Count == BlueTeamPlayers.Count;
            public abstract TeamColor LosingTeam { get; }

            private static IEnumerable<UserResult> GetTeamResults(List<BattleTankPlayer> players) => players.Where(x => x.MatchPlayer != null).Select(x => x.MatchPlayer.UserResult);
            public IEnumerable<UserResult> RedTeamResults => GetTeamResults(RedTeamPlayers);
            public IEnumerable<UserResult> BlueTeamResults => GetTeamResults(BlueTeamPlayers);

            public virtual BattleView BattleViewFor(BattleTankPlayer battlePlayer)
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
            public int EnemyCountFor(BattleTankPlayer battlePlayer) => BattleViewFor(battlePlayer).EnemyTeamPlayers.Count;
            public TeamBattleResult TeamBattleResultFor(BattleTankPlayer battlePlayer) => (RedTeamEntity.GetComponent<TeamScoreComponent>().Score - BlueTeamEntity.GetComponent<TeamScoreComponent>().Score) switch
            {
                > 0 => battlePlayer.Team == RedTeamEntity ? TeamBattleResult.WIN : TeamBattleResult.DEFEAT,
                < 0 => battlePlayer.Team == BlueTeamEntity ? TeamBattleResult.WIN : TeamBattleResult.DEFEAT,
                _ => TeamBattleResult.DRAW
            };

            public void SortRoundUsers()
            {
                foreach (List<BattleTankPlayer> list in new[] { RedTeamPlayers, BlueTeamPlayers })
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

                SpawnPoints = teamModesSpawnPoints.ContainsKey(Battle.Params.BattleMode)
                    ? teamModesSpawnPoints[Battle.Params.BattleMode]
                    : teamModesSpawnPoints.Values.First(b => b != null);
            }

            public void SetupBattle(IBattleModeHandler prevHandler)
            {
                SetupBattle();

                if (prevHandler is TeamBattleHandler teamBattleHandler)
                {
                    RedTeamPlayers = teamBattleHandler.RedTeamPlayers;
                    BlueTeamPlayers = teamBattleHandler.BlueTeamPlayers;

                    foreach (BattleTankPlayer battlePlayer in Players)
                        battlePlayer.Team = battlePlayer.Team.GetComponent<TeamColorComponent>().TeamColor == TeamColor.RED ? RedTeamEntity : BlueTeamEntity;
                }
                else
                {
                    foreach (BattleTankPlayer battlePlayer in prevHandler.Players)
                    {
                        Entity teamEntity;
                        List<BattleTankPlayer> teamPlayerList;

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

            public BattleTankPlayer AddPlayer(Player player)
            {
                List<BattleTankPlayer> teamPlayerList = null;
                Entity teamEntity = null;

                List<List<BattleTankPlayer>> teamPlayerLists = new() { RedTeamPlayers, BlueTeamPlayers };
                Dictionary<List<BattleTankPlayer>, Entity> entityPlayersDict = new()
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

                BattleTankPlayer battlePlayer = new(Battle, player, teamEntity);
                teamPlayerList.Add(battlePlayer);

                player.BattlePlayer = battlePlayer;
                return battlePlayer;
            }

            public void RemovePlayer(BattleTankPlayer battlePlayer)
            {
                if (!RedTeamPlayers.Remove(battlePlayer))
                    BlueTeamPlayers.Remove(battlePlayer);

                battlePlayer.User.RemoveComponent<TeamColorComponent>();
                battlePlayer.Player.BattlePlayer = null;
            }

            public virtual void OnMatchJoin(BaseBattlePlayer battlePlayer)
            {
                battlePlayer.ShareEntities(RedTeamEntity, BlueTeamEntity, TeamBattleChatEntity);
            }

            public virtual void OnMatchLeave(BaseBattlePlayer battlePlayer)
            {
                battlePlayer.UnshareEntities(BlueTeamEntity, RedTeamEntity, TeamBattleChatEntity);
            }
        }
    }
}
