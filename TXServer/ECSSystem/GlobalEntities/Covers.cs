using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Covers 
    {
        public static Items GlobalItems { get; } = new Items();

        public static Items GetUserItems(Entity user)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.EntityId = Player.GenerateId();

                item.TemplateAccessor.Template = new WeaponPaintUserItemTemplate();

                item.Components.Add(new UserGroupComponent(user.EntityId));
            }

            items.None.Components.Add(new MountedItemComponent());

            return items;
        }

        public class Items : ItemList
        {
            public Entity Antaeus1 { get; } = new Entity(-891349912, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/antaeus1"),
                new MarketItemGroupComponent(-891349912));
            public Entity Antaeus2018 { get; } = new Entity(1577640270, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/antaeus2018"),
                new MarketItemGroupComponent(1577640270));
            public Entity Blue { get; } = new Entity(-172609771, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/blue"),
                new MarketItemGroupComponent(-172609771));
            public Entity Carbon { get; } = new Entity(1644116328, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/carbon"),
                new MarketItemGroupComponent(1644116328));
            public Entity Ceder { get; } = new Entity(-1055236844, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/ceder"),
                new MarketItemGroupComponent(-1055236844));
            public Entity Champion { get; } = new Entity(888876452, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/champion"),
                new MarketItemGroupComponent(888876452));
            public Entity Coal { get; } = new Entity(-172577710, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/coal"),
                new MarketItemGroupComponent(-172577710));
            public Entity Corrosion { get; } = new Entity(-849655395, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/corrosion"),
                new MarketItemGroupComponent(-849655395));
            public Entity Desert { get; } = new Entity(1676472336, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/desert"),
                new MarketItemGroupComponent(1676472336));
            public Entity Digital { get; } = new Entity(534589821, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/digital"),
                new MarketItemGroupComponent(534589821));
            public Entity Dirt { get; } = new Entity(-172553150, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/dirt"),
                new MarketItemGroupComponent(-172553150));
            public Entity Dragon { get; } = new Entity(1687943694, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/dragon"),
                new MarketItemGroupComponent(1687943694));
            public Entity Flame { get; } = new Entity(-1052260392, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/flame"),
                new MarketItemGroupComponent(-1052260392));
            public Entity Flora { get; } = new Entity(-1052246787, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/flora"),
                new MarketItemGroupComponent(-1052246787));
            public Entity Forester { get; } = new Entity(-75661627, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/forester"),
                new MarketItemGroupComponent(-75661627));
            public Entity Frontier1 { get; } = new Entity(610004612, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/frontier1"),
                new MarketItemGroupComponent(610004612));
            public Entity Frontier2018 { get; } = new Entity(640844210, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/frontier2018"),
                new MarketItemGroupComponent(640844210));
            public Entity Galaxy { get; } = new Entity(-64859568, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/galaxy"),
                new MarketItemGroupComponent(-64859568));
            public Entity Glacier { get; } = new Entity(-1705840017, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/glacier"),
                new MarketItemGroupComponent(-1705840017));
            public Entity Glina { get; } = new Entity(-1051329156, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/glina"),
                new MarketItemGroupComponent(-1051329156));
            public Entity Gold { get; } = new Entity(-172458213, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/gold"),
                new MarketItemGroupComponent(-172458213));
            public Entity Green { get; } = new Entity(-1051154520, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/green"),
                new MarketItemGroupComponent(-1051154520));
            public Entity Greenskulls { get; } = new Entity(1953107513, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/greenskulls"),
                new MarketItemGroupComponent(1953107513));
            public Entity Halloween { get; } = new Entity(-1128248718, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/halloween"),
                new MarketItemGroupComponent(-1128248718));
            public Entity Honeycomb { get; } = new Entity(-784118404, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/honeycomb"),
                new MarketItemGroupComponent(-784118404));
            public Entity Hvoya { get; } = new Entity(-1050101618, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/hvoya"),
                new MarketItemGroupComponent(-1050101618));
            public Entity Inferno { get; } = new Entity(819242484, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/inferno"),
                new MarketItemGroupComponent(819242484));
            public Entity Izumrud { get; } = new Entity(372065215, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/izumrud"),
                new MarketItemGroupComponent(372065215));
            public Entity Lightblue { get; } = new Entity(1167717168, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/lightblue"),
                new MarketItemGroupComponent(1167717168));
            public Entity Marine { get; } = new Entity(1930414525, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/marine"),
                new MarketItemGroupComponent(1930414525));
            public Entity Mary { get; } = new Entity(-172292714, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/mary"),
                new MarketItemGroupComponent(-172292714));
            public Entity Matteblack { get; } = new Entity(1958622537, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/matteblack"),
                new MarketItemGroupComponent(1958622537));
            public Entity Matteblue { get; } = new Entity(1448655312, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/matteblue"),
                new MarketItemGroupComponent(1448655312));
            public Entity Mattegray { get; } = new Entity(1448809433, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/mattegray"),
                new MarketItemGroupComponent(1448809433));
            public Entity Mattegreen { get; } = new Entity(1963422797, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/mattegreen"),
                new MarketItemGroupComponent(1963422797));
            public Entity Mattenavi { get; } = new Entity(1449002268, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/mattenavi"),
                new MarketItemGroupComponent(1449002268));
            public Entity Matteorange { get; } = new Entity(965487140, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/matteorange"),
                new MarketItemGroupComponent(965487140));
            public Entity Mattered { get; } = new Entity(-230348709, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/mattered"),
                new MarketItemGroupComponent(-230348709));
            public Entity Matterose { get; } = new Entity(1449134789, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/matterose"),
                new MarketItemGroupComponent(1449134789));
            public Entity Matteviolet { get; } = new Entity(1157994613, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/matteviolet"),
                new MarketItemGroupComponent(1157994613));
            public Entity Mattewhite { get; } = new Entity(1977905523, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/mattewhite"),
                new MarketItemGroupComponent(1977905523));
            public Entity Matteyellow { get; } = new Entity(1240098922, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/matteyellow"),
                new MarketItemGroupComponent(1240098922));
            public Entity May2018_1 { get; } = new Entity(-217437764, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/may2018_1"),
                new MarketItemGroupComponent(-217437764));
            public Entity May2018_2 { get; } = new Entity(-217437763, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/may2018_2"),
                new MarketItemGroupComponent(-217437763));
            public Entity Metel { get; } = new Entity(-1045986264, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/metel"),
                new MarketItemGroupComponent(-1045986264));
            public Entity Nefrit { get; } = new Entity(1962388777, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/nefrit"),
                new MarketItemGroupComponent(1962388777));
            public Entity Newyearglow { get; } = new Entity(1273539023, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/newyearglow"),
                new MarketItemGroupComponent(1273539023));
            public Entity Newyearice { get; } = new Entity(-97463799, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/newyearice"),
                new MarketItemGroupComponent(-97463799));
            public Entity Noise { get; } = new Entity(-1044774977, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/noise"),
                new MarketItemGroupComponent(-1044774977));
            public Entity None { get; } = new Entity(-172249613, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/none"),
                new MarketItemGroupComponent(-172249613));
            public Entity North { get; } = new Entity(-1044766294, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/north"),
                new MarketItemGroupComponent(-1044766294));
            public Entity Orange { get; } = new Entity(2002870825, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/orange"),
                new MarketItemGroupComponent(2002870825));
            public Entity Partizan { get; } = new Entity(645983436, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/partizan"),
                new MarketItemGroupComponent(645983436));
            public Entity Patina { get; } = new Entity(2016361556, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/patina"),
                new MarketItemGroupComponent(2016361556));
            public Entity Pink { get; } = new Entity(-172195791, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/pink"),
                new MarketItemGroupComponent(-172195791));
            public Entity Polar { get; } = new Entity(-1042925597, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/polar"),
                new MarketItemGroupComponent(-1042925597));
            public Entity Priboy { get; } = new Entity(2031727040, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/priboy"),
                new MarketItemGroupComponent(2031727040));
            public Entity Prodigy { get; } = new Entity(-1435376695, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/prodigy"),
                new MarketItemGroupComponent(-1435376695));
            public Entity Radiation { get; } = new Entity(-500584127, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/radiation"),
                new MarketItemGroupComponent(-500584127));
            public Entity Red { get; } = new Entity(410089078, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/red"),
                new MarketItemGroupComponent(410089078));
            public Entity Rivets { get; } = new Entity(2081063968, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/rivets"),
                new MarketItemGroupComponent(2081063968));
            public Entity Rocks { get; } = new Entity(-1041086893, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/rocks"),
                new MarketItemGroupComponent(-1041086893));
            public Entity Rust { get; } = new Entity(-172124513, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/rust"),
                new MarketItemGroupComponent(-172124513));
            public Entity Sandstone { get; } = new Entity(2014177414, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/sandstone"),
                new MarketItemGroupComponent(2014177414));
            public Entity Savanna { get; } = new Entity(746819053, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/savanna"),
                new MarketItemGroupComponent(746819053));
            public Entity Smoke { get; } = new Entity(-1040211436, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/smoke"),
                new MarketItemGroupComponent(-1040211436));
            public Entity Sport { get; } = new Entity(-1040121831, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/sport"),
                new MarketItemGroupComponent(-1040121831));
            public Entity Starrysky { get; } = new Entity(-1779740531, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/starrysky"),
                new MarketItemGroupComponent(-1779740531));
            public Entity Steamulatorf { get; } = new Entity(1408168452, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/steamulatorf"),
                new MarketItemGroupComponent(1408168452));
            public Entity Steamulatorsf { get; } = new Entity(703549557, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/steamulatorsf"),
                new MarketItemGroupComponent(703549557));
            public Entity Steel { get; } = new Entity(-1040012688, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/steel"),
                new MarketItemGroupComponent(-1040012688));
            public Entity Stone { get; } = new Entity(-1040002806, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/stone"),
                new MarketItemGroupComponent(-1040002806));
            public Entity Storm { get; } = new Entity(-1040002674, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/storm"),
                new MarketItemGroupComponent(-1040002674));
            public Entity Stvalentine2017 { get; } = new Entity(-26936066, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/stvalentine2017"),
                new MarketItemGroupComponent(-26936066));
            public Entity Swamp { get; } = new Entity(-1039926907, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/swamp"),
                new MarketItemGroupComponent(-1039926907));
            public Entity Taiga { get; } = new Entity(-1039651301, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/taiga"),
                new MarketItemGroupComponent(-1039651301));
            public Entity Tar { get; } = new Entity(410090890, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/tar"),
                new MarketItemGroupComponent(410090890));
            public Entity Tina { get; } = new Entity(-172076637, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/tina"),
                new MarketItemGroupComponent(-172076637));
            public Entity Trianglecamo { get; } = new Entity(-2097840184, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/trianglecamo"),
                new MarketItemGroupComponent(-2097840184));
            public Entity Tundra { get; } = new Entity(-2145802143, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/tundra"),
                new MarketItemGroupComponent(-2145802143));
            public Entity Universe { get; } = new Entity(176253763, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/universe"),
                new MarketItemGroupComponent(176253763));
            public Entity Urban { get; } = new Entity(-1038228233, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/urban"),
                new MarketItemGroupComponent(-1038228233));
            public Entity Vostok1 { get; } = new Entity(-487032800, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/vostok1"),
                new MarketItemGroupComponent(-487032800));
            public Entity Walker { get; } = new Entity(-2078438351, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/walker"),
                new MarketItemGroupComponent(-2078438351));
            public Entity Yellow { get; } = new Entity(454169524, new TemplateAccessor(new WeaponPaintMarketItemTemplate(), "garage/cover/yellow"),
                new MarketItemGroupComponent(454169524));
        }
    }
}
