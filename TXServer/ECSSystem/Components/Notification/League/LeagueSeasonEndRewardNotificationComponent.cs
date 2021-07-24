using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.League;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components.Notification.League
{
    [SerialVersionUID(1508753065310L)]
    public class LeagueSeasonEndRewardNotificationComponent : Component
    {
        public LeagueSeasonEndRewardNotificationComponent(Player player)
        {
            SeasonNumber = player.ServerData.SeasonNumber - 1;
            LeagueId = player.Data.LastSeasonLeagueId;

            // master/top graffiti reward
            if (new[] {Leagues.GlobalItems.Gold.EntityId, Leagues.GlobalItems.Master.EntityId}.Contains(player.Data
                    .LastSeasonLeagueId) &&
                player.ServerData.SeasonGraffities.TryGetValue(player.ServerData.SeasonNumber - 1,
                    out (long _, long topId) graffiti) &&
                player.HasEntityWithId(graffiti.topId, out Entity topGraffiti) &&
                !player.Data.OwnsMarketItem(topGraffiti))
            {
                Reward.Add(graffiti.topId, 1);
                player.SaveNewMarketItem(topGraffiti);
            }

            // usual league rewards
            foreach (DroppedItem reward in CurrentSeasonRewardForClientComponent.GetLeagueRewards(player.Data
                .LastSeasonLeagueIndex).SelectMany(info => info.Items))
            {
                switch (player.Data.LastSeasonLeagueIndex)
                {
                    case 3:
                        if (reward.MarketItemEntity.TemplateAccessor.Template.GetType() ==
                            typeof(XCrystalMarketItemTemplate))
                            reward.Amount = player.Data.LastSeasonLeaguePlace < 51 ? 500 : 400;
                        break;
                    case 4:
                        if (reward.MarketItemEntity.EntityId == Paints.GlobalItems.Hero.EntityId &&
                            player.Data.LastSeasonLeaguePlace != 1)
                            continue;
                        reward.Amount = reward.MarketItemEntity.TemplateAccessor.Template switch
                        {
                            GameplayChestMarketItemTemplate => player.Data.LastSeasonLeaguePlace switch
                            {
                                < 4 => 20,
                                > 3 and < 51 => 15,
                                _ => 10
                            },
                            XCrystalMarketItemTemplate => player.Data.LastSeasonLeaguePlace switch
                            {
                                1 => 9999,
                                2 or 3 => 2000,
                                > 3 and < 11 => 1500,
                                > 10 and < 52 => 1000,
                                _ => 750
                            },
                            _ => reward.Amount
                        };
                        break;
                }

                Reward.Add(reward.MarketItemEntity.EntityId, reward.Amount);
                player.SaveNewMarketItem(reward.MarketItemEntity, reward.Amount);
            }

            player.Data.ReceivedLastSeasonReward = true;
        }

        public int SeasonNumber { get; set; }
        public long LeagueId { get; set; }
        public Dictionary<long, int> Reward { get; set; } = new();
    }
}
