using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Round
{
    [SerialVersionUID(6921761768819133913)]
    public class RoundUserStatisticsComponent : Component
    {
        public RoundUserStatisticsComponent(int place, int scoreWithoutBonuses, int kills, int killAssists, int deaths)
        {
            Place = place;
            ScoreWithoutBonuses = scoreWithoutBonuses;
            Kills = kills;
            KillAssists = killAssists;
            Deaths = deaths;
        }

        public RoundUserStatisticsComponent() => Place = ScoreWithoutBonuses = Kills = KillAssists = Deaths = 0;

        public int Place { get; set; }

        public int ScoreWithoutBonuses { get; set; }

        public int Kills { get; set; }

        public int KillAssists { get; set; }

        public int Deaths { get; set; }
    }
}
