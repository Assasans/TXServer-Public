using System.Collections.Generic;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Fraction;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Notification.FractionsCompetition;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Fractions
    {
        public static Items GlobalItems { get; } = new Items();

        public static Dictionary<int, Entity> ByIndex { get; } = new Dictionary<int, Entity>()
        {
            [0] = GlobalItems.Antaeus,
            [1] = GlobalItems.Frontier
        };

        public static Dictionary<Entity, int> ToId { get; } = new Dictionary<Entity, int>()
        {
            [GlobalItems.Antaeus] = 0,
            [GlobalItems.Frontier] = 1
        };

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

        public static void CheckForNotifications(Player player)
        {
            // fractions competition start notification
            if (ServerData.FractionsCompetitionActive && !ServerData.FractionsCompetitionFinished &&
                !player.Data.ShowedFractionsCompetition)
                player.ShareEntities(FractionsCompetitionStartNotificationTemplate.CreateEntity(player));

            // update fraction scores
            if (ServerData.FractionsCompetitionActive || ServerData.FractionsCompetitionFinished)
                player.UpdateFractionScores();
        }

        private static Entity GetWinner() => ServerData.AntaeusScore > ServerData.FrontierScore
                ? GlobalItems.Antaeus
                : GlobalItems.Frontier;

        private static ServerData ServerData => Server.Instance.ServerData;
    }
}
