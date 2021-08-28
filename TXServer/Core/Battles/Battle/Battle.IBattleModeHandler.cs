using System.Collections.Generic;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Events.Battle.Score;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public interface IBattleModeHandler
        {
            Battle Battle { get; init; }

            void SetupBattle();
            void Tick();

            IEnumerable<BattleTankPlayer> Players { get; }
            bool IsEnoughPlayers { get; }
            TeamColor LosingTeam { get; }
            int EnemyCountFor(BattleTankPlayer battlePlayer);
            TeamBattleResult TeamBattleResultFor(BattleTankPlayer battlePlayer);
            void SortRoundUsers();
            void SortRoundUsers(List<BattleTankPlayer> list)
            {
                list.Sort(new ScoreComparer());

                int place = 1;
                foreach (BattleTankPlayer battlePlayer in list.ToArray())
                {
                    if (battlePlayer.MatchPlayer == null) continue;

                    Entity roundUser = battlePlayer.MatchPlayer.RoundUser;
                    var component = roundUser.GetComponent<RoundUserStatisticsComponent>();

                    if (component.Place != place)
                    {
                        component.Place = place;
                        roundUser.ChangeComponent(component);
                        Battle.PlayersInMap.SendEvent(new SetScoreTablePositionEvent(place), roundUser);
                    }

                    place++;
                }
            }

            void SetupBattle(IBattleModeHandler prevHandler);
            void CompleteWarmUp();
            void OnFinish();

            BattleTankPlayer AddPlayer(Player player);
            void RemovePlayer(BattleTankPlayer battlePlayer);
            void OnMatchJoin(BaseBattlePlayer battlePlayer);
            void OnMatchLeave(BaseBattlePlayer battlePlayer);
        }
    }
}
