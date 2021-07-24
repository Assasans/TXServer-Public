using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Components.Notification.League
{
    [SerialVersionUID(1505906112954L)]
    public class LeagueFirstEntranceRewardNotificationComponent : Component
    {
        public LeagueFirstEntranceRewardNotificationComponent(Player player)
        {
            player.Data.RewardedLeagues.Add(player.Data.League.EntityId);

            Reward = player.Data.League.EntityId switch
            {
                -101377070 => new Dictionary<long, int> {{Containers.GlobalItems.Cardsbronze.EntityId, 1}},
                2119734820 => new Dictionary<long, int> {{Containers.GlobalItems.Cardssilver.EntityId, 1}},
                414840278 => new Dictionary<long, int> {{Containers.GlobalItems.Cardsgold.EntityId, 1}},
                1131431735 => new Dictionary<long, int> {{Containers.GlobalItems.Cardsmaster.EntityId, 1}},
                _ => new Dictionary<long, int>()
            };

            foreach ((long rewardId, int amount) in Reward)
                player.SaveNewMarketItem(player.EntityList.Single(e => e.EntityId == rewardId), amount);
        }

        private readonly Dictionary<long, int> _bronzeReward = new()
        {
            { ExtraItems.GlobalItems.Crystal.EntityId, 100 }
        };
        private readonly Dictionary<long, int> _silverReward = new()
        {
            { ExtraItems.GlobalItems.Crystal.EntityId, 100 }
        };
        private readonly Dictionary<long, int> _goldReward = new()
        {
            { ExtraItems.GlobalItems.Crystal.EntityId, 100 }
        };
        private readonly Dictionary<long, int> _masterReward = new()
        {
            { ExtraItems.GlobalItems.Crystal.EntityId, 100 }
        };

        public Dictionary<long, int> Reward { get; set; }
    }
}
