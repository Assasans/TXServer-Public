using System.Reflection;
using System.Runtime.Serialization;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Paints : ItemList
    {
        public static Paints GlobalItems { get; } = new Paints();

        public static ItemList GetUserItems(Entity user)
        {
            ItemList items = FormatterServices.GetUninitializedObject(typeof(Paints)) as ItemList;

            foreach (PropertyInfo info in typeof(Paints).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity _23feb2019 { get; private set; } = new Entity(300183087, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/23feb2019"),
            new MarketItemGroupComponent(300183087));
        public Entity _8march19_1 { get; private set; } = new Entity(685548944, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/8march19-1"),
            new MarketItemGroupComponent(685548944));
        public Entity _8march19_2 { get; private set; } = new Entity(685548945, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/8march19-2"),
            new MarketItemGroupComponent(685548945));
        public Entity _8march19_3 { get; private set; } = new Entity(685548946, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/8march19-3"),
            new MarketItemGroupComponent(685548946));
        public Entity Anavatan { get; private set; } = new Entity(231787227, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/anaVatan"),
            new MarketItemGroupComponent(231787227));
        public Entity Antaeus1 { get; private set; } = new Entity(1104561449, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/antaeus1"),
            new MarketItemGroupComponent(1104561449));
        public Entity Antaeus2018 { get; private set; } = new Entity(1104561450, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/antaeus2018"),
            new MarketItemGroupComponent(1104561450));
        public Entity Antarctida { get; private set; } = new Entity(683803510, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/antarctida"),
            new MarketItemGroupComponent(683803510));
        public Entity Arrowsoflove { get; private set; } = new Entity(-400341658, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/arrowsoflove"),
            new MarketItemGroupComponent(-400341658));
        public Entity Autumnleaves { get; private set; } = new Entity(-662236117, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/autumnleaves"),
            new MarketItemGroupComponent(-662236117));
        public Entity Bananas { get; private set; } = new Entity(-574655849, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/bananas"),
            new MarketItemGroupComponent(-574655849));
        public Entity Beginning { get; private set; } = new Entity(-1500327034, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/beginning"),
            new MarketItemGroupComponent(-1500327034));
        public Entity Birthday2017 { get; private set; } = new Entity(776075330, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/birthday2017"),
            new MarketItemGroupComponent(776075330));
        public Entity Blackroger { get; private set; } = new Entity(2008328619, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/blackroger"),
            new MarketItemGroupComponent(2008328619));
        public Entity Blue { get; private set; } = new Entity(137747283, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/blue"),
            new MarketItemGroupComponent(137747283));
        public Entity Bluemagma { get; private set; } = new Entity(436923060, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/bluemagma"),
            new MarketItemGroupComponent(436923060));
        public Entity Canyon { get; private set; } = new Entity(214261055, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/canyon"),
            new MarketItemGroupComponent(214261055));
        public Entity Carbon { get; private set; } = new Entity(214261056, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/carbon"),
            new MarketItemGroupComponent(214261056));
        public Entity Ceder { get; private set; } = new Entity(-24102762, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/ceder"),
            new MarketItemGroupComponent(-24102762));
        public Entity Champion { get; private set; } = new Entity(1798212322, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/champion"),
            new MarketItemGroupComponent(1798212322));
        public Entity Coal { get; private set; } = new Entity(920905258, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/coal"),
            new MarketItemGroupComponent(920905258));
        public Entity Corrosion { get; private set; } = new Entity(1569952799, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/corrosion"),
            new MarketItemGroupComponent(1569952799));
        public Entity Desert { get; private set; } = new Entity(-718109490, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/desert"),
            new MarketItemGroupComponent(-718109490));
        public Entity Digital { get; private set; } = new Entity(-683002753, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/digital"),
            new MarketItemGroupComponent(-683002753));
        public Entity Dirt { get; private set; } = new Entity(137803904, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/dirt"),
            new MarketItemGroupComponent(137803904));
        public Entity Dragon { get; private set; } = new Entity(-706638132, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/dragon"),
            new MarketItemGroupComponent(-706638132));
        public Entity Easternstar { get; private set; } = new Entity(1717585789, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/easternstar"),
            new MarketItemGroupComponent(1717585789));
        public Entity Epicgold { get; private set; } = new Entity(-892221448, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/epicgold"),
            new MarketItemGroupComponent(-892221448));
        public Entity Flame { get; private set; } = new Entity(-1514026752, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/flame"),
            new MarketItemGroupComponent(-1514026752));
        public Entity Flora { get; private set; } = new Entity(-21112705, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/flora"),
            new MarketItemGroupComponent(-21112705));
        public Entity Football { get; private set; } = new Entity(761115110, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/football"),
            new MarketItemGroupComponent(761115110));
        public Entity Forester { get; private set; } = new Entity(833674243, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/forester"),
            new MarketItemGroupComponent(833674243));
        public Entity Freedom { get; private set; } = new Entity(1347685821, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/freedom"),
            new MarketItemGroupComponent(1347685821));
        public Entity Frontier1 { get; private set; } = new Entity(-1396691349, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/frontier1"),
            new MarketItemGroupComponent(-1396691349));
        public Entity Frontier2018 { get; private set; } = new Entity(-1396691350, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/frontier2018"),
            new MarketItemGroupComponent(-1396691350));
        public Entity Galaxy { get; private set; } = new Entity(-636128311, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/galaxy"),
            new MarketItemGroupComponent(-636128311));
        public Entity Glacier { get; private set; } = new Entity(526265313, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/glacier"),
            new MarketItemGroupComponent(526265313));
        public Entity Glina { get; private set; } = new Entity(-20195074, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/glina"),
            new MarketItemGroupComponent(-20195074));
        public Entity Gold { get; private set; } = new Entity(921024755, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/gold"),
            new MarketItemGroupComponent(921024755));
        public Entity Green { get; private set; } = new Entity(-20020438, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/green"),
            new MarketItemGroupComponent(-20020438));
        public Entity Greenskulls { get; private set; } = new Entity(97943767, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/greenskulls"),
            new MarketItemGroupComponent(97943767));
        public Entity Halloween { get; private set; } = new Entity(721518013, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/halloween"),
            new MarketItemGroupComponent(721518013));
        public Entity Halloween2017 { get; private set; } = new Entity(1218811098, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/halloween2017"),
            new MarketItemGroupComponent(1218811098));
        public Entity Hero { get; private set; } = new Entity(137919219, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/hero"),
            new MarketItemGroupComponent(137919219));
        public Entity Honeycomb { get; private set; } = new Entity(1023813172, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/honeycomb"),
            new MarketItemGroupComponent(1023813172));
        public Entity Hvoya { get; private set; } = new Entity(-18967536, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/hvoya"),
            new MarketItemGroupComponent(-18967536));
        public Entity Hydra { get; private set; } = new Entity(-887676103, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/hydra"),
            new MarketItemGroupComponent(-887676103));
        public Entity Inferno { get; private set; } = new Entity(-398350090, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/inferno"),
            new MarketItemGroupComponent(-398350090));
        public Entity League_bronze { get; private set; } = new Entity(1185540727, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/league_bronze"),
            new MarketItemGroupComponent(1185540727));
        public Entity League_gold { get; private set; } = new Entity(-1245546371, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/league_gold"),
            new MarketItemGroupComponent(-1245546371));
        public Entity League_master { get; private set; } = new Entity(-317558887, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/league_master"),
            new MarketItemGroupComponent(-317558887));
        public Entity League_silver { get; private set; } = new Entity(1663842282, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/league_silver"),
            new MarketItemGroupComponent(1663842282));
        public Entity Marine { get; private set; } = new Entity(-464167301, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/marine"),
            new MarketItemGroupComponent(-464167301));
        public Entity Mary { get; private set; } = new Entity(138064340, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/mary"),
            new MarketItemGroupComponent(138064340));
        public Entity Matteblack { get; private set; } = new Entity(-646190818, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/matteblack"),
            new MarketItemGroupComponent(-646190818));
        public Entity Matteblue { get; private set; } = new Entity(-436467641, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/matteblue"),
            new MarketItemGroupComponent(-436467641));
        public Entity Mattegray { get; private set; } = new Entity(-431689890, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/mattegray"),
            new MarketItemGroupComponent(-431689890));
        public Entity Mattegreen { get; private set; } = new Entity(-497382758, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/mattegreen"),
            new MarketItemGroupComponent(-497382758));
        public Entity Mattenavi { get; private set; } = new Entity(-425712005, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/mattenavi"),
            new MarketItemGroupComponent(-425712005));
        public Entity Matteorange { get; private set; } = new Entity(267667571, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/matteorange"),
            new MarketItemGroupComponent(267667571));
        public Entity Mattered { get; private set; } = new Entity(-1260536244, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/mattered"),
            new MarketItemGroupComponent(-1260536244));
        public Entity Matterose { get; private set; } = new Entity(-421603854, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/matterose"),
            new MarketItemGroupComponent(-421603854));
        public Entity Matteviolet { get; private set; } = new Entity(1940431938, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/matteviolet"),
            new MarketItemGroupComponent(1940431938));
        public Entity Mattewhite { get; private set; } = new Entity(-48418252, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/mattewhite"),
            new MarketItemGroupComponent(-48418252));
        public Entity Matteyellow { get; private set; } = new Entity(190698221, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/matteyellow"),
            new MarketItemGroupComponent(190698221));
        public Entity May2018_1 { get; private set; } = new Entity(2070833571, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/may2018_1"),
            new MarketItemGroupComponent(2070833571));
        public Entity May2018_2 { get; private set; } = new Entity(2070833572, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/may2018_2"),
            new MarketItemGroupComponent(2070833572));
        public Entity Metel { get; private set; } = new Entity(-14852182, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/metel"),
            new MarketItemGroupComponent(-14852182));
        public Entity Military { get; private set; } = new Entity(-1660396512, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/military"),
            new MarketItemGroupComponent(-1660396512));
        public Entity Mosaic { get; private set; } = new Entity(-451216061, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/mosaic"),
            new MarketItemGroupComponent(-451216061));
        public Entity Moscow { get; private set; } = new Entity(-1878474135, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/moscow"),
            new MarketItemGroupComponent(-1878474135));
        public Entity Nefrit { get; private set; } = new Entity(532533505, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/nefrit"),
            new MarketItemGroupComponent(532533505));
        public Entity Newyearglow { get; private set; } = new Entity(-1760088841, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/newyearglow"),
            new MarketItemGroupComponent(-1760088841));
        public Entity Newyearice { get; private set; } = new Entity(-888059423, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/newyearice"),
            new MarketItemGroupComponent(-888059423));
        public Entity Noise { get; private set; } = new Entity(-1506541337, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/noise"),
            new MarketItemGroupComponent(-1506541337));
        public Entity North { get; private set; } = new Entity(-1506532654, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/north"),
            new MarketItemGroupComponent(-1506532654));
        public Entity Orange { get; private set; } = new Entity(-391711001, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/orange"),
            new MarketItemGroupComponent(-391711001));
        public Entity Orion { get; private set; } = new Entity(-12628116, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/orion"),
            new MarketItemGroupComponent(-12628116));
        public Entity Partizan { get; private set; } = new Entity(1555319306, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/partizan"),
            new MarketItemGroupComponent(1555319306));
        public Entity Patina { get; private set; } = new Entity(-1079898253, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/patina"),
            new MarketItemGroupComponent(-1079898253));
        public Entity Pink { get; private set; } = new Entity(921287177, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/pink"),
            new MarketItemGroupComponent(921287177));
        public Entity Polar { get; private set; } = new Entity(-1504691957, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/polar"),
            new MarketItemGroupComponent(-1504691957));
        public Entity Priboy { get; private set; } = new Entity(-362854786, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/priboy"),
            new MarketItemGroupComponent(-362854786));
        public Entity Prodigy { get; private set; } = new Entity(1641998027, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/prodigy"),
            new MarketItemGroupComponent(1641998027));
        public Entity Radiation { get; private set; } = new Entity(1223441167, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/radiation"),
            new MarketItemGroupComponent(1223441167));
        public Entity Red { get; private set; } = new Entity(558647928, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/red"),
            new MarketItemGroupComponent(558647928));
        public Entity Redamber { get; private set; } = new Entity(1784687185, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/redamber"),
            new MarketItemGroupComponent(1784687185));
        public Entity Rivets { get; private set; } = new Entity(925876519, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/rivets"),
            new MarketItemGroupComponent(925876519));
        public Entity Rocks { get; private set; } = new Entity(-9952811, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/rocks"),
            new MarketItemGroupComponent(-9952811));
        public Entity Rust { get; private set; } = new Entity(276536681, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/rust"),
            new MarketItemGroupComponent(276536681));
        public Entity Sandstone { get; private set; } = new Entity(138818312, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/sandstone"),
            new MarketItemGroupComponent(138818312));
        public Entity Savanna { get; private set; } = new Entity(-470773521, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/savanna"),
            new MarketItemGroupComponent(-470773521));
        public Entity Smoke { get; private set; } = new Entity(-9077354, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/smoke"),
            new MarketItemGroupComponent(-9077354));
        public Entity Spectre { get; private set; } = new Entity(-56970637, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/spectre"),
            new MarketItemGroupComponent(-56970637));
        public Entity Spiderweb { get; private set; } = new Entity(355636982, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/spiderweb"),
            new MarketItemGroupComponent(355636982));
        public Entity Sport { get; private set; } = new Entity(-1207082244, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/sport"),
            new MarketItemGroupComponent(-1207082244));
        public Entity Starrysky { get; private set; } = new Entity(-1732490966, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/starrysky"),
            new MarketItemGroupComponent(-1732490966));
        public Entity Steamulatorf { get; private set; } = new Entity(1880146981, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/steamulatorf"),
            new MarketItemGroupComponent(1880146981));
        public Entity Steamulatorsf { get; private set; } = new Entity(-1844971488, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/steamulatorsf"),
            new MarketItemGroupComponent(-1844971488));
        public Entity Steel { get; private set; } = new Entity(-1501779048, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/steel"),
            new MarketItemGroupComponent(-1501779048));
        public Entity Stone { get; private set; } = new Entity(-1756632159, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/stone"),
            new MarketItemGroupComponent(-1756632159));
        public Entity Storm { get; private set; } = new Entity(-8868592, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/storm"),
            new MarketItemGroupComponent(-8868592));
        public Entity Stvalentine2017 { get; private set; } = new Entity(986557478, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/stvalentine2017"),
            new MarketItemGroupComponent(986557478));
        public Entity Summer { get; private set; } = new Entity(-274067352, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/summer"),
            new MarketItemGroupComponent(-274067352));
        public Entity Swamp { get; private set; } = new Entity(-8792825, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/swamp"),
            new MarketItemGroupComponent(-8792825));
        public Entity Taiga { get; private set; } = new Entity(-8517219, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/taiga"),
            new MarketItemGroupComponent(-8517219));
        public Entity Tar { get; private set; } = new Entity(-1209537658, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/tar"),
            new MarketItemGroupComponent(-1209537658));
        public Entity Tina { get; private set; } = new Entity(138280417, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/tina"),
            new MarketItemGroupComponent(138280417));
        public Entity Trianglecamo { get; private set; } = new Entity(-1902859834, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/trianglecamo"),
            new MarketItemGroupComponent(-1902859834));
        public Entity Trollface { get; private set; } = new Entity(1546602523, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/trollface"),
            new MarketItemGroupComponent(1546602523));
        public Entity Tundra { get; private set; } = new Entity(-245416673, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/tundra"),
            new MarketItemGroupComponent(-245416673));
        public Entity Universe { get; private set; } = new Entity(942805628, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/universe"),
            new MarketItemGroupComponent(942805628));
        public Entity Urban { get; private set; } = new Entity(-7094151, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/urban"),
            new MarketItemGroupComponent(-7094151));
        public Entity Vostok1 { get; private set; } = new Entity(-2023428617, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/vostok1"),
            new MarketItemGroupComponent(-2023428617));
        public Entity Walker { get; private set; } = new Entity(-50932577, new TemplateAccessor(new TankPaintMarketItemTemplate(), "garage/paint/walker"),
            new MarketItemGroupComponent(-50932577));
    }
}
