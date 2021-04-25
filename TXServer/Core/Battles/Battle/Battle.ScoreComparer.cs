using System.Collections.Generic;
using TXServer.ECSSystem.Components.Battle.Round;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        private class ScoreComparer : IComparer<BattlePlayer>
        {
            public int Compare(BattlePlayer x, BattlePlayer y)
            {
                int xv = x.MatchPlayer?.RoundUser.GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses ?? 0;
                int yv = y.MatchPlayer?.RoundUser.GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses ?? 0;
                return yv.CompareTo(xv);
            }
        }
    }
}
