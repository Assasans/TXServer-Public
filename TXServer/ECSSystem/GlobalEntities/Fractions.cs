using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Fraction;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Fractions
    {
        public static Items GlobalItems { get; } = new Items();

        public class Items : ItemList
        {
            public Entity Competition
            {
                get
                {
                    Entity competition = new(1529751975, new TemplateAccessor(new FractionsCompetitionTemplate(),
                            "fractionscompetition"));
                    if (ServerData.FractionsCompetitionFinished)
                        competition.AddComponent(new FinishedFractionsCompetitionComponent(GetWinner().EntityId));
                    return competition;
                }
            }

            public Entity Antaeus
            {
                get
                {
                    Entity antaeus = new(-1650120701, new TemplateAccessor(new FractionTemplate(),
                            "fractionscompetition/fractions/antaeus"),
                        new FractionGroupComponent(-1650120701),
                        new FractionComponent("antaeus"));
                    if (ServerData.FractionsCompetitionActive)
                        antaeus.AddComponent(new FractionInvolvedInCompetitionComponent(ServerData.AntaeusUserCount));
                    return antaeus;
                }
            }

            public Entity Frontier
            {
                get
                {
                    Entity frontier = new(-372139284, new TemplateAccessor(new FractionTemplate(),
                            "fractionscompetition/fractions/frontier"),
                        new FractionGroupComponent(-372139284),
                        new FractionComponent("frontier"));
                    if (ServerData.FractionsCompetitionActive)
                        frontier.AddComponent(new FractionInvolvedInCompetitionComponent(ServerData.FrontierUserCount));

                    return frontier;
                }
            }
        }

        private static Entity GetWinner() => ServerData.AntaeusScore > ServerData.FrontierScore
                ? GlobalItems.Antaeus
                : GlobalItems.Frontier;

        private static ServerData ServerData => Server.Instance.ServerData;
    }
}
