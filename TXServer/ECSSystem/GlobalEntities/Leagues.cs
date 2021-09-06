using System.Collections.Generic;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.BattleRewards;
using TXServer.ECSSystem.Components.League;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Notification.League;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Leagues
    {
        public static Items GlobalItems { get; } = new Items();

        public static Dictionary<int, Entity> ByIndex { get; } = new Dictionary<int, Entity>()
        {
            [0] = GlobalItems.Training,
            [1] = GlobalItems.Bronze,
            [2] = GlobalItems.Silver,
            [3] = GlobalItems.Gold,
            [4] = GlobalItems.Master
        };

        public class Items : ItemList
        {
            public Entity Training { get; } = new Entity(-1837531149, new TemplateAccessor(new LeagueTemplate(), "leagues/leagues/1_training"),
                new ChestBattleRewardComponent(Containers.GlobalItems.Cardsbronze),
                new LeagueGroupComponent(-1837531149),
                new CurrentSeasonRewardForClientComponent(0),
                new LeagueConfigComponent(0, 100));
            public Entity Bronze { get; } = new Entity(-101377070, new TemplateAccessor(new LeagueTemplate(), "leagues/leagues/2_bronze"),
                new ChestBattleRewardComponent(Containers.GlobalItems.Cardsbronze),
                new LeagueGroupComponent(-101377070),
                new CurrentSeasonRewardForClientComponent(1),
                new LeagueConfigComponent(1, 140));
            public Entity Silver { get; } = new Entity(2119734820, new TemplateAccessor(new LeagueTemplate(), "leagues/leagues/3_silver"),
                new ChestBattleRewardComponent(Containers.GlobalItems.Cardssilver),
                new LeagueGroupComponent(2119734820),
                new CurrentSeasonRewardForClientComponent(2),
                new LeagueConfigComponent(2, 1000));
            public Entity Gold { get; } = new Entity(414840278, new TemplateAccessor(new LeagueTemplate(), "leagues/leagues/4_gold"),
                new ChestBattleRewardComponent(Containers.GlobalItems.Cardsgold),
                new LeagueGroupComponent(414840278),
                new CurrentSeasonRewardForClientComponent(3),
                new LeagueConfigComponent(3, 3000));
            public Entity Master { get; } = new Entity(1131431735, new TemplateAccessor(new LeagueTemplate(), "leagues/leagues/5_master"),
                new ChestBattleRewardComponent(Containers.GlobalItems.Cardsmaster),
                new LeagueGroupComponent(1131431735),
                new CurrentSeasonRewardForClientComponent(4),
                new LeagueConfigComponent(4, 4500));
        }

        public static void CheckForNotifications(Player player)
        {
            // season first entrance reward
            if (!player.Data.RewardedLeagues.ContainsId(player.Data.League.EntityId) &&
                player.Data.League.EntityId != GlobalItems.Training.EntityId)
                player.ShareEntities(LeagueFirstEntranceRewardPersistentNotificationTemplate.CreateEntity(player));

            // finished season reward
            if (player.ServerData.SpreadLastSeasonRewards && !player.Data.ReceivedLastSeasonReward &&
                player.Data.LastSeasonBattles > 0 && player.Data.LastSeasonLeagueIndex > 0)
                player.ShareEntities(LeagueSeasonEndRewardPersistentNotificationTemplate.CreateEntity(player));

            // season normal graffiti
            if (player.ServerData.SeasonGraffities.TryGetValue(player.ServerData.SeasonNumber,
                    out (long normalId, long _) graffiti) &&
                player.HasEntityWithId(graffiti.normalId, out Entity graffitiReward) &&
                !player.Data.OwnsMarketItem(graffitiReward) &&
                player.Data.Statistics.BattlesParticipatedInSeason > 9)
                player.SaveNewMarketItem(graffitiReward);
        }
    }
}
