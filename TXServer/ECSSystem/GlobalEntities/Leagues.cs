using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Leagues : ItemList
    {
        public static Leagues GlobalItems { get; } = new Leagues();

        public Entity Training { get; } = LeagueTemplate.CreateEntity(-1837531149, "1_training", Containers.GlobalItems.Cardsbronze, 0);
        public Entity Bronze { get; } = LeagueTemplate.CreateEntity(-101377070, "2_bronze", Containers.GlobalItems.Cardsbronze, 1);
        public Entity Silver { get; } = LeagueTemplate.CreateEntity(2119734820, "3_silver", Containers.GlobalItems.Cardssilver, 2);
        public Entity Gold { get; } = LeagueTemplate.CreateEntity(414840278, "4_gold", Containers.GlobalItems.Cardsgold, 3);
        public Entity Master { get; } = LeagueTemplate.CreateEntity(1131431735, "5_master", Containers.GlobalItems.Cardsmaster, 4);
    }
}
