using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public class DMHandler : IBattleModeHandler
        {
            public Battle Battle { get; init; }

            public IEnumerable<BattleTankPlayer> Players => _Players;
            private List<BattleTankPlayer> _Players = new();

            public IEnumerable<UserResult> Results => Players.Where(x => x.MatchPlayer != null).Select(x => x.MatchPlayer.UserResult);
            public int EnemyCountFor(BattleTankPlayer battlePlayer) => _Players.Count - Convert.ToInt32(_Players.Contains(battlePlayer));
            public TeamBattleResult TeamBattleResultFor(BattleTankPlayer battlePlayer) => TeamBattleResult.DRAW;
            public void SortRoundUsers() => ((IBattleModeHandler)this).SortRoundUsers(_Players);

            public bool IsEnoughPlayers => _Players.Count > 2;
            public TeamColor LosingTeam => TeamColor.NONE;

            public IList<SpawnPoint> SpawnPoints { get; private set; }

            public void SetupBattle()
            {
                SpawnPoints = Battle.CurrentMapInfo.SpawnPoints.Deathmatch;
            }

            public void SetupBattle(IBattleModeHandler prevHandler)
            {
                SetupBattle();

                _Players = prevHandler.Players.ToList();

                foreach (BattleTankPlayer battlePlayer in prevHandler.Players)
                {
                    battlePlayer.Team = null;
                    battlePlayer.Player.User.ChangeComponent(new TeamColorComponent(TeamColor.NONE));
                }
            }

            public void CompleteWarmUp()
            {
            }

            public void OnFinish()
            {
            }

            public void Tick()
            {
            }

            public BattleTankPlayer AddPlayer(Player player)
            {
                BattleTankPlayer battlePlayer = new(Battle, player, null);
                player.BattlePlayer = battlePlayer;
                
                player.User.AddComponent(new TeamColorComponent(TeamColor.NONE));

                _Players.Add(battlePlayer);

                return battlePlayer;
            }

            public void RemovePlayer(BattleTankPlayer battlePlayer)
            {
                _Players.Remove(battlePlayer);

                battlePlayer.User.RemoveComponent<TeamColorComponent>();
                battlePlayer.Player.BattlePlayer = null;
            }

            public void OnMatchJoin(BaseBattlePlayer battlePlayer)
            {
            }

            public void OnMatchLeave(BaseBattlePlayer battlePlayer)
            {
            }
        }
    }
}
