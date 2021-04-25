using System.Collections.Generic;
using System.Linq;
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

            IEnumerable<BattlePlayer> Players { get; }
            bool IsEnoughPlayers { get; }
            TeamColor LosingTeam { get; }
            int EnemyCountFor(BattlePlayer battlePlayer);
            TeamBattleResult TeamBattleResultFor(BattlePlayer battlePlayer);
            void SortRoundUsers();
            void SortRoundUsers(List<BattlePlayer> list)
            {
                list.Sort(new ScoreComparer());

                int place = 1;
                foreach (BattlePlayer battlePlayer in list)
                {
                    if (battlePlayer.MatchPlayer == null) continue;

                    Entity roundUser = battlePlayer.MatchPlayer.RoundUser;
                    var component = roundUser.GetComponent<RoundUserStatisticsComponent>();

                    if (component.Place != place)
                    {
                        component.Place = place;
                        roundUser.ChangeComponent(component);
                        Battle.MatchPlayers.Select(x => x.Player).SendEvent(new SetScoreTablePositionEvent(place), roundUser);
                    }

                    place++;
                }
            }

            void SetupBattle(IBattleModeHandler prevHandler);
            void CompleteWarmUp();
            void OnFinish();

            BattlePlayer AddPlayer(Player player, bool isSpectator);
            void RemovePlayer(BattlePlayer battlePlayer);
            void OnMatchJoin(BattlePlayer battlePlayer);
            void OnMatchLeave(BattlePlayer battlePlayer);
        }
    }
}
