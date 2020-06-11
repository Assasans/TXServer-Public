using System.Reflection;
using System.Runtime.Serialization;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Containers : ItemList
    {
        public static Containers GlobalItems { get; } = new Containers();

        public static Containers GetUserItems(Entity user)
        {
            Containers items = FormatterServices.GetUninitializedObject(typeof(Containers)) as Containers;
            
            foreach (PropertyInfo info in typeof(Containers).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity _12april2017 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(759177625, "12april2017");
        public Entity _12april2018 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(100024310, "12april2018");
        public Entity _12april2019 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(763642357, "12april2019");
        public Entity _2018 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(158184859, "2018");
        public Entity _23february2017 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(782261271, "23february2017");
        public Entity _23february2018 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(782261272, "23february2018");
        public Entity _23february2019 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(647046527, "23february2019");
        public Entity _8march2017 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-382197697, "8march2017");
        public Entity _8march2019 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-2044621657, "8march2019");
        public Entity Avatars { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(160097023, "avatars");
        public Entity Birthday2017paints { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(1616681926, "birthday2017paints");
        public Entity Birthday2017skins { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-1191706491, "birthday2017skins");
        public Entity Camouflage { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(255419373, "camouflage");
        public Entity Cardsbronze { get; private set; } = GameplayChestMarketItemTemplate.CreateEntity(-370755132, "cardsbronze");
        public Entity Cardsgold { get; private set; } = GameplayChestMarketItemTemplate.CreateEntity(-1147355315, "cardsgold");
        public Entity Cardsgolddonut { get; private set; } = DonutChestMarketItemTemplate.CreateEntity(-1667426315, "cardsgolddonut");
        public Entity Cardsmaster { get; private set; } = GameplayChestMarketItemTemplate.CreateEntity(1357210127, "cardsmaster");
        public Entity Cardsscout { get; private set; } = DonutChestMarketItemTemplate.CreateEntity(1357210027, "cardsscout");
        public Entity Cardssilver { get; private set; } = GameplayChestMarketItemTemplate.CreateEntity(1536166586, "cardssilver");
        public Entity Cardssilverdonut { get; private set; } = DonutChestMarketItemTemplate.CreateEntity(-1308563288, "cardssilverdonut");
        public Entity Cardsspydonut { get; private set; } = DonutChestMarketItemTemplate.CreateEntity(1060751187, "cardsspydonut");
        public Entity Cover { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(655960513, "cover");
        public Entity Crystallight { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(208629334, "crystallight");
        public Entity Everything { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(1933847273, "everything");
        public Entity Frontierzero { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(1185680389, "frontierzero");
        public Entity Gold { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-678260688, "gold");
        public Entity Halloween2018 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-208273732, "halloween2018");
        public Entity Mars { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(159038379, "Mars");
        public Entity May2017 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-1096656045, "may2017");
        public Entity May20181 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-1619467770, "may2018_1");
        public Entity May20182 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-1619467769, "may2018_2");
        public Entity May20183 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-1619467768, "may2018_3");
        public Entity Prey { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(160097004, "prey");
        public Entity Rubber { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(1346673024, "rubber");
        public Entity Shadowassassin { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(145730912, "shadowassassin");
        public Entity Smoke { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(670670713, "smoke");
        public Entity Steam { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-1462392318, "steam");
        public Entity Steel { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(459979771, "steel");
        public Entity Superenergy { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-1301708723, "superenergy");
        public Entity Supportantaeus { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-1191706490, "supportantaeus");
        public Entity Supportantaeus1 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(2132723585, "supportantaeus1");
        public Entity Supportfrontier { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(1616681925, "supportfrontier");
        public Entity Supportfrontier1 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(-132997493, "supportfrontier1");
        public Entity Titanxt { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(1046779026, "titanXT");
        public Entity Tutorialbronze1 { get; private set; } = TutorialGameplayChestMarketItemTemplate.CreateEntity(-1415877053, "tutorialbronze1");
        public Entity Tutorialbronze2 { get; private set; } = TutorialGameplayChestMarketItemTemplate.CreateEntity(-1415877052, "tutorialbronze2");
        public Entity Twinsxt { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(1437820497, "twinsXT");
        public Entity Valhalla { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(185937013, "valhalla");
        public Entity Vikingt2 { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(1325772382, "vikingT2");
        public Entity Xmas { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(285430311, "xmas");
        public Entity Xt { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(742065458, "xt");
        public Entity Xtzeus { get; private set; } = ContainerPackPriceMarketItemTemplate.CreateEntity(598090230, "xt_zeus");
    }
}
