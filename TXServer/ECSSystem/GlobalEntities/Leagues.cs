using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Leagues
    {
        public static Items GlobalItems { get; } = new Items();

        public class Items : ItemList
        {
            public Entity Training { get; } = new Entity(-1837531149, new TemplateAccessor(new LeagueTemplate(), "leagues/leagues/1_training"),
                new ChestBattleRewardComponent(Containers.GlobalItems.Cardsbronze),
                new LeagueGroupComponent(-1837531149),
                new CurrentSeasonRewardForClientComponent(),
                new LeagueConfigComponent(0, 100));
            public Entity Bronze { get; } = new Entity(-101377070, new TemplateAccessor(new LeagueTemplate(), "leagues/leagues/2_bronze"),
                new ChestBattleRewardComponent(Containers.GlobalItems.Cardsbronze),
                new LeagueGroupComponent(-101377070),
                new CurrentSeasonRewardForClientComponent(),
                new LeagueConfigComponent(1, 140));
            public Entity Silver { get; } = new Entity(2119734820, new TemplateAccessor(new LeagueTemplate(), "leagues/leagues/3_silver"),
                new ChestBattleRewardComponent(Containers.GlobalItems.Cardssilver),
                new LeagueGroupComponent(2119734820),
                new CurrentSeasonRewardForClientComponent(),
                new LeagueConfigComponent(2, 1000));
            public Entity Gold { get; } = new Entity(414840278, new TemplateAccessor(new LeagueTemplate(), "leagues/leagues/4_gold"),
                new ChestBattleRewardComponent(Containers.GlobalItems.Cardsgold),
                new LeagueGroupComponent(414840278),
                new CurrentSeasonRewardForClientComponent(),
                new LeagueConfigComponent(3, 3000));
            public Entity Master { get; } = new Entity(1131431735, new TemplateAccessor(new LeagueTemplate(), "leagues/leagues/5_master"),
                new ChestBattleRewardComponent(Containers.GlobalItems.Cardsmaster),
                new LeagueGroupComponent(1131431735),
                new CurrentSeasonRewardForClientComponent(),
                new LeagueConfigComponent(4, 4500));
        }
    }
}
