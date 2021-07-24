using System;
using System.Collections.Generic;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Core
{
    public class ServerData
    {
        public ServerData()
        {
            // todo: load data from database
        }

        public bool FractionsCompetitionActive { get; set; }
        public bool FractionsCompetitionFinished { get; set; }
        public long FractionsCompetitionCryFund => (AntaeusScore + FrontierScore) * 3;
        public long AntaeusScore
        {
            get => _antaeusScore;
            set
            {
                _antaeusScore = value;
                foreach (Player player in Server.Instance.Connection.Pool)
                    player.UpdateFractionScores();
            }
        }
        public long AntaeusUserCount { get; set; }
        public long FrontierScore
        {
            get => _frontierScore;
            set
            {
                _frontierScore = value;
                foreach (Player player in Server.Instance.Connection.Pool)
                    player.UpdateFractionScores();
            }
        }
        public long FrontierUserCount { get; set; }

        public bool SpreadLastSeasonRewards { get; set; }
        public int SeasonNumber { get; set; }

        public bool SpreadReleaseGift { get; set; }
        public DateTimeOffset ReleaseGiftMaxRegistrationDate { get; set; } = DateTimeOffset.UtcNow;


        private long _antaeusScore;
        private long _frontierScore;


        public readonly Dictionary<int, (long, long)> SeasonGraffities = new()
        {
            { 0, (Graffiti.GlobalItems.Season0.EntityId, Graffiti.GlobalItems.Season0top.EntityId) },
            { 1, (Graffiti.GlobalItems.Season1.EntityId, Graffiti.GlobalItems.Season1top.EntityId) },
            { 2, (Graffiti.GlobalItems.Season2.EntityId, Graffiti.GlobalItems.Season2top.EntityId) },
            { 3, (Graffiti.GlobalItems.Season3_ny2018.EntityId, Graffiti.GlobalItems.Season3_ny2018top.EntityId) },
            { 4, (Graffiti.GlobalItems.Season3.EntityId, Graffiti.GlobalItems.Season3top.EntityId) },
            { 5, (Graffiti.GlobalItems.Season4.EntityId, Graffiti.GlobalItems.Season4top.EntityId) },
            { 6, (Graffiti.GlobalItems.Season5.EntityId, Graffiti.GlobalItems.Season5top.EntityId) },
            { 7, (Graffiti.GlobalItems.Season6.EntityId, Graffiti.GlobalItems.Season6top.EntityId) },
            { 8, (Graffiti.GlobalItems.Season7.EntityId, Graffiti.GlobalItems.Season7top.EntityId) },
            { 9, (Graffiti.GlobalItems.Season8.EntityId, Graffiti.GlobalItems.Season8top.EntityId) },
            { 10, (Graffiti.GlobalItems.Season9.EntityId, Graffiti.GlobalItems.Season9top.EntityId) },
            { 11, (Graffiti.GlobalItems.Season10.EntityId, Graffiti.GlobalItems.Season10top.EntityId) },
            { 12, (Graffiti.GlobalItems.Season11.EntityId, Graffiti.GlobalItems.Season11top.EntityId) },
            { 13, (Graffiti.GlobalItems.Season12.EntityId, Graffiti.GlobalItems.Season12top.EntityId) }
        };
    }
}
