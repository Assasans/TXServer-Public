using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Components.Notification
{
    [SerialVersionUID(636564551794907676L)]
    public class ReleaseGiftsNotificationComponent : Component
    {
        public ReleaseGiftsNotificationComponent(Player player)
        {
            foreach ((long rewardId, int amount) in Reward)
            {
                Entity rewardMarketItem = player.EntityList.Single(e => e.EntityId == rewardId);
                player.SaveNewMarketItem(rewardMarketItem, amount);
            }

            player.Data.ReceivedReleaseReward = true;
        }

        public Dictionary<long, int> Reward { get; set; } = new()
        {
            { Paints.GlobalItems.Beginning.EntityId, 1 },
            { Containers.GlobalItems.Cardssilverdonut.EntityId, 10 },
            { ExtraItems.GlobalItems.Xcrystal.EntityId, 350 }
        };
    }
}
