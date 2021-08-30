using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TXServer.Core;
using TXServer.Core.ContainerInfo;
using TXServer.Core.ShopContainers;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Containers
    {
        public static Items GlobalItems { get; } = new Items();
        public static Items GetUserItems(Player player)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                long id = item.EntityId;
                item.EntityId = Entity.GenerateId();

                switch (item.TemplateAccessor.Template)
                {
                    case ContainerPackPriceMarketItemTemplate _:
                        item.TemplateAccessor.Template = new ContainerUserItemTemplate();
                        break;
                    case GameplayChestMarketItemTemplate _:
                        item.TemplateAccessor.Template = new GameplayChestUserItemTemplate();
                        break;
                    case TutorialGameplayChestMarketItemTemplate _:
                        item.TemplateAccessor.Template = new TutorialGameplayChestUserItemTemplate();
                        break;
                    case DonutChestMarketItemTemplate _:
                        item.TemplateAccessor.Template = new SimpleChestUserItemTemplate();
                        break;
                }

                item.Components.Remove(new RestrictionByUserFractionComponent());
                item.Components.Add(new UserGroupComponent(player.User));

                player.Data.Containers.TryGetById(id, container => container.Count, out int amount);
                item.Components.Add(new UserItemCounterComponent(amount));
                item.Components.Add(new NotificationGroupComponent(item.EntityId));
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity _12april2017 { get; } = new Entity(759177625, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/12april2017"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(759177625));
            public Entity _12april2018 { get; } = new Entity(100024310, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/12april2018"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(100024310));
            public Entity _12april2019 { get; } = new Entity(763642357, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/12april2019"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(763642357));
            public Entity _2018 { get; } = new Entity(158184859, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/2018"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(158184859));
            public Entity _23february2017 { get; } = new Entity(782261271, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/23february2017"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(782261271));
            public Entity _23february2018 { get; } = new Entity(782261272, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/23february2018"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(782261272));
            public Entity _23february2019 { get; } = new Entity(647046527, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/23february2019"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(647046527));
            public Entity _8march2017 { get; } = new Entity(-382197697, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/8march2017"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-382197697));
            public Entity _8march2019 { get; } = new Entity(-2044621657, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/8march2019"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-2044621657));
            public Entity Avatars { get; } = new Entity(160097023, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/avatars"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(160097023));
            public Entity Birthday2017paints { get; } = new Entity(1616681926, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/birthday2017paints"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(1616681926));
            public Entity Birthday2017skins { get; } = new Entity(-1191706491, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/birthday2017skins"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-1191706491));
            public Entity Camouflage { get; } = new Entity(255419373, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/camouflage"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(255419373));
            public Entity Cardsbronze { get; } = new Entity(-370755132, new TemplateAccessor(new GameplayChestMarketItemTemplate(), "garage/container/cardsbronze"),
                new MarketItemGroupComponent(-370755132));
            public Entity Cardsgold { get; } = new Entity(-1147355315, new TemplateAccessor(new GameplayChestMarketItemTemplate(), "garage/container/cardsgold"),
                new MarketItemGroupComponent(-1147355315));
            public Entity Cardsgolddonut { get; } = new Entity(-1667426315, new TemplateAccessor(new DonutChestMarketItemTemplate(), "garage/container/cardsgolddonut"),
                new MarketItemGroupComponent(-1667426315));
            public Entity Cardsmaster { get; } = new Entity(1357210127, new TemplateAccessor(new GameplayChestMarketItemTemplate(), "garage/container/cardsmaster"),
                new MarketItemGroupComponent(1357210127));
            public Entity Cardsscout { get; } = new Entity(1357210027, new TemplateAccessor(new DonutChestMarketItemTemplate(), "garage/container/cardsscout"),
                new MarketItemGroupComponent(1357210027));
            public Entity Cardssilver { get; } = new Entity(1536166586, new TemplateAccessor(new GameplayChestMarketItemTemplate(), "garage/container/cardssilver"),
                new MarketItemGroupComponent(1536166586));
            public Entity Cardssilverdonut { get; } = new Entity(-1308563288, new TemplateAccessor(new DonutChestMarketItemTemplate(), "garage/container/cardssilverdonut"),
                new MarketItemGroupComponent(-1308563288));
            public Entity Cardsspydonut { get; } = new Entity(1060751187, new TemplateAccessor(new DonutChestMarketItemTemplate(), "garage/container/cardsspydonut"),
                new MarketItemGroupComponent(1060751187));
            public Entity Cover { get; } = new Entity(655960513, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/cover"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(655960513));
            public Entity Crystallight { get; } = new Entity(208629334, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/crystallight"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(208629334));
            public Entity Everything { get; } = new Entity(1933847273, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/everything"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(1933847273));
            public Entity Frontierzero { get; } = new Entity(1185680389, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/frontierzero"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(1185680389));
            public Entity Gold { get; } = new Entity(-678260688, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/gold"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-678260688));
            public Entity Halloween2018 { get; } = new Entity(-208273732, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/halloween2018"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-208273732));
            public Entity Mars { get; } = new Entity(159038379, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/Mars"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(159038379));
            public Entity May2017 { get; } = new Entity(-1096656045, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/may2017"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-1096656045));
            public Entity May2018_1 { get; } = new Entity(-1619467770, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/may2018_1"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-1619467770));
            public Entity May2018_2 { get; } = new Entity(-1619467769, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/may2018_2"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-1619467769));
            public Entity May2018_3 { get; } = new Entity(-1619467768, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/may2018_3"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-1619467768));
            public Entity Prey { get; } = new Entity(160097004, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/prey"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(160097004));
            public Entity Rubber { get; } = new Entity(1346673024, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/rubber"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(1346673024));
            public Entity Shadowassassin { get; } = new Entity(145730912, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/shadowassassin"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(145730912));
            public Entity Smoke { get; } = new Entity(670670713, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/smoke"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(670670713));
            public Entity Steam { get; } = new Entity(-1462392318, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/steam"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-1462392318));
            public Entity Steel { get; } = new Entity(459979771, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/steel"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(459979771));
            public Entity Superenergy { get; } = new Entity(-1301708723, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/superenergy"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-1301708723));
            public Entity Supportantaeus { get; } = new Entity(-1191706490, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/supportantaeus"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-1191706490));
            public Entity Supportantaeus1 { get; } = new Entity(2132723585, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/supportantaeus1"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(2132723585));
            public Entity Supportfrontier { get; } = new Entity(1616681925, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/supportfrontier"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(1616681925));
            public Entity Supportfrontier1 { get; } = new Entity(-132997493, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/supportfrontier1"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(-132997493));
            public Entity Titanxt { get; } = new Entity(1046779026, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/titanXT"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(1046779026));
            public Entity Tutorialbronze1 { get; } = new Entity(-1415877053, new TemplateAccessor(new TutorialGameplayChestMarketItemTemplate(), "garage/container/tutorialbronze1"),
                new MarketItemGroupComponent(-1415877053));
            public Entity Tutorialbronze2 { get; } = new Entity(-1415877052, new TemplateAccessor(new TutorialGameplayChestMarketItemTemplate(), "garage/container/tutorialbronze2"),
                new MarketItemGroupComponent(-1415877052));
            public Entity Twinsxt { get; } = new Entity(1437820497, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/twinsXT"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(1437820497));
            public Entity Valhalla { get; } = new Entity(185937013, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/valhalla"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(185937013));
            public Entity Vikingt2 { get; } = new Entity(1325772382, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/vikingT2"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(1325772382));
            public Entity Xmas { get; } = new Entity(285430311, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/xmas"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(285430311));
            public Entity Xt { get; } = new Entity(742065458, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/xt"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(742065458));
            public Entity Xt_zeus { get; } = new Entity(598090230, new TemplateAccessor(new ContainerPackPriceMarketItemTemplate(), "garage/container/xt_zeus"),
                new RestrictionByUserFractionComponent(),
                new MarketItemGroupComponent(598090230));
        }

        private static readonly Dictionary<Entity, Type> ContainerToType = new()
        {
            { GlobalItems.Everything, typeof(EverythingContainer) },
            { GlobalItems.Cardsscout, typeof(ScoutContainer) }
        };

        public static ShopContainer GetShopContainer(Entity containerMarketItem, Player player)
        {
            if (ContainerToType.ContainsKey(containerMarketItem))
                return (ShopContainer) Activator.CreateInstance(ContainerToType[containerMarketItem],
                    containerMarketItem, player, new ContainerInfo());

            ContainerInfo containerInfo = ServerConnection.ContainerInfos
                .SingleOrDefault(i => i.Key == containerMarketItem.TemplateAccessor.ConfigPath.Split("/").Last()).Value;

            if (containerInfo is not null)
                return new BlueprintContainer(containerMarketItem, player, containerInfo);

            return new StandardItemContainer(containerMarketItem, player, new ContainerInfo());
        }
    }
}
