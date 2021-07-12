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
        public CurrentSeasonRewardForClientComponent(int leagueIndex)
        {
            List<DroppedItem> items = new List<DroppedItem>();
            switch (leagueIndex)
            {
                case 0:
                    Rewards.Add(new EndSeasonRewardItem(100, 139, items));
                    break;
                case 1:
                    items.Add(new DroppedItem(Containers.GlobalItems.Cardsbronze, 20));
                    items.Add(new DroppedItem(ExtraItems.GlobalItems.Crystal, 20000));
                    items.Add(new DroppedItem(Paints.GlobalItems.League_bronze, 1));
                    Rewards.Add(new EndSeasonRewardItem(140, 999, items));
                    break;
                case 2:
                    items.Add(new DroppedItem(Containers.GlobalItems.Cardssilver, 20));
                    items.Add(new DroppedItem(ExtraItems.GlobalItems.Crystal, 60000));
                    items.Add(new DroppedItem(Paints.GlobalItems.League_silver, 1));
                    Rewards.Add(new EndSeasonRewardItem(1000, 2999, items));
                    break;
                case 3:
                    items.Add(new DroppedItem(Containers.GlobalItems.Cardsgold, 20));
                    items.Add(new DroppedItem(ExtraItems.GlobalItems.Xcrystal, 500));
                    items.Add(new DroppedItem(Paints.GlobalItems.League_gold, 1));
                    Rewards.Add(new EndSeasonRewardItem(3000, 4499, items));
                    break;
                case 4:
                    items.Add(new DroppedItem(Containers.GlobalItems.Cardsmaster, 20));
                    items.Add(new DroppedItem(Containers.GlobalItems.Xt_zeus, 1));
                    items.Add(new DroppedItem(ExtraItems.GlobalItems.Xcrystal, 9999));
                    items.Add(new DroppedItem(Paints.GlobalItems.League_master, 1));
                    items.Add(new DroppedItem(Paints.GlobalItems.Hero, 1));
                    Rewards.Add(new EndSeasonRewardItem(4500, 99999, items));
                    break;
            }
        }

        public List<EndSeasonRewardItem> Rewards { get; set; } = new();
    }
}
