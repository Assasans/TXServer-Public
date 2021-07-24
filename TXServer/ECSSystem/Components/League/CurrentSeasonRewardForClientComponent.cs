using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components.League
{
    [SerialVersionUID(1503654626834L)]
    public class CurrentSeasonRewardForClientComponent : Component
    {
        public CurrentSeasonRewardForClientComponent(int leagueIndex) => Rewards = GetLeagueRewards(leagueIndex);

        public static List<EndSeasonRewardItem> GetLeagueRewards(int leagueIndex)
        {
            return leagueIndex switch
            {
                0 => new List<EndSeasonRewardItem>{new(100, 139, new List<DroppedItem>())},
                1 => new List<EndSeasonRewardItem>{new(140, 999, BronzeRewards)},
                2 => new List<EndSeasonRewardItem>{new(1000, 2999, SilverRewards)},
                3 => new List<EndSeasonRewardItem>{new(3000, 4499, GoldRewards)},
                4 => new List<EndSeasonRewardItem>{new(4500, 99999, MasterRewards)},
                _ => new List<EndSeasonRewardItem>()
            };
        }

        private static readonly List<DroppedItem> BronzeRewards = new()
        {
            new DroppedItem(Containers.GlobalItems.Cardsbronze, 20),
            new DroppedItem(ExtraItems.GlobalItems.Crystal, 20000),
            new DroppedItem(Paints.GlobalItems.League_bronze, 1)
        };
        private static readonly List<DroppedItem> SilverRewards = new()
        {
            new DroppedItem(Containers.GlobalItems.Cardssilver, 20),
            new DroppedItem(ExtraItems.GlobalItems.Crystal, 60000),
            new DroppedItem(Paints.GlobalItems.League_silver, 1)
        };
        private static readonly List<DroppedItem> GoldRewards = new()
        {
            new DroppedItem(Containers.GlobalItems.Cardsgold, 20),
            new DroppedItem(ExtraItems.GlobalItems.Xcrystal, 500),
            new DroppedItem(Paints.GlobalItems.League_gold, 1)
        };
        private static readonly List<DroppedItem> MasterRewards = new()
        {
            new DroppedItem(Containers.GlobalItems.Cardsmaster, 20),
            new DroppedItem(Containers.GlobalItems.Xt_zeus, 1),
            new DroppedItem(ExtraItems.GlobalItems.Xcrystal, 9999),
            new DroppedItem(Paints.GlobalItems.League_master, 1),
            new DroppedItem(Paints.GlobalItems.Hero, 1)
        };

        public List<EndSeasonRewardItem> Rewards { get; set; }
    }
}
