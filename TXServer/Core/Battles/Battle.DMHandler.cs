using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public class DMHandler : IBattleModeHandler
        {
            public Battle Battle { get; init; }

            public IEnumerable<BattlePlayer> Players => _Players;
            private List<BattlePlayer> _Players = new();

            public IEnumerable<UserResult> Results => Players.Where(x => x.MatchPlayer != null).Select(x => x.MatchPlayer.UserResult);
            public int EnemyCountFor(BattlePlayer battlePlayer) => _Players.Count - Convert.ToInt32(_Players.Contains(battlePlayer));
            public TeamBattleResult TeamBattleResultFor(BattlePlayer battlePlayer) => TeamBattleResult.DRAW;

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

                foreach (BattlePlayer battlePlayer in prevHandler.Players)
                {
                    battlePlayer.Team = null;
                    battlePlayer.Player.User.ChangeComponent(new TeamColorComponent(TeamColor.NONE));
                }
            }

            public void CompleteWarmUp()
            {
            }

            public void Tick()
            {
            }

            public BattlePlayer AddPlayer(Player player)
            {
                player.User.AddComponent(new TeamColorComponent(TeamColor.NONE));

                BattlePlayer battlePlayer = new(Battle, player, null);
                player.BattlePlayer = battlePlayer;

                _Players.Add(battlePlayer);

                return battlePlayer;
            }

            public void RemovePlayer(BattlePlayer battlePlayer)
            {
                _Players.Remove(battlePlayer);

                battlePlayer.User.RemoveComponent<TeamColorComponent>();
                battlePlayer.Player.BattlePlayer = null;
            }

            public void OnMatchJoin(BattlePlayer battlePlayer)
            {
            }

            public void OnMatchLeave(BattlePlayer battlePlayer)
            {
            }
        }
    }
}
