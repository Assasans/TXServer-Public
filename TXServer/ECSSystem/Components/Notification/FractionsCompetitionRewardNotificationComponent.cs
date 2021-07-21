using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Components.Notification
{
    [SerialVersionUID(1547017351411L)]
    public class FractionsCompetitionRewardNotificationComponent : Component
    {
        public FractionsCompetitionRewardNotificationComponent(Player player)
        {
            long winnerScore;
            Dictionary<long, (long, int)> rewards;

            (winnerScore, rewards, WinnerFractionId) = ServerData.FrontierScore > ServerData.AntaeusScore
                ? (ServerData.FrontierScore, _frontierRewards, Fractions.GlobalItems.Frontier.EntityId)
                : (ServerData.AntaeusScore, _antaeusRewards, Fractions.GlobalItems.Antaeus.EntityId);

            CrysForWin = ServerData.FractionsCompetitionCryFund;

            foreach ((long neededScore, (long rewardId, int amount)) in rewards)
                if (winnerScore >= neededScore)
                    Rewards.Add(rewardId, amount);

            // save rewards if user in winning fraction
            if (player.Data.Fraction.EntityId == WinnerFractionId)
            {
                foreach ((long rewardId, int amount) in Rewards)
                {
                    Entity reward = player.EntityList.Single(e => e.EntityId == rewardId);
                    player.SaveNewMarketItem(reward, amount);
                }
            }

            player.Data.ReceivedFractionsCompetitionReward = true;
        }

        private static ServerData ServerData => Server.Instance.ServerData;

        private readonly Dictionary<long, (long, int)> _antaeusRewards = new()
        {
            { 100000, (ExtraItems.GlobalItems.Goldbonus.EntityId, 20) },
            { 500000, (Containers.GlobalItems.Cardsspydonut.EntityId, 2) },
            { 1000000, (ExtraItems.GlobalItems.Crystal.EntityId, 50000) },
            { 1500000, (ExtraItems.GlobalItems.Premiumboost.EntityId, 10) },
            { 2000000, (Containers.GlobalItems.Birthday2017skins.EntityId, 5) }
        };
        private readonly Dictionary<long, (long, int)> _frontierRewards = new()
        {
            { 50000, (ExtraItems.GlobalItems.Premiumboost.EntityId, 3) },
            { 300000, (ExtraItems.GlobalItems.Crystal.EntityId, 20000) },
            { 550000, (ExtraItems.GlobalItems.Goldbonus.EntityId, 5) },
            { 800000, (Containers.GlobalItems.Cardsscout.EntityId, 1) },
            { 1100000, (ExtraItems.GlobalItems.Premiumboost.EntityId, 7) },
            { 1400000, (Containers.GlobalItems.Cardsspydonut.EntityId, 1) },
            { 1750000, (ExtraItems.GlobalItems.Goldbonus.EntityId, 15) },
            { 2000000, (Containers.GlobalItems.Birthday2017paints.EntityId, 8) }
        };

        public long WinnerFractionId { get; set; }
        public long CrysForWin { get; set; }
        public Dictionary<long, int> Rewards { get; set; } = new();
    }
}
