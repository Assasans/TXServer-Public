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

        public Entity _23feb2019 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(300183087, "23feb2019");
        public Entity _8march191 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(685548944, "8march19-1");
        public Entity _8march192 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(685548945, "8march19-2");
        public Entity _8march193 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(685548946, "8march19-3");
        public Entity Anavatan { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(231787227, "anaVatan");
        public Entity Antaeus1 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1104561449, "antaeus1");
        public Entity Antaeus2018 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1104561450, "antaeus2018");
        public Entity Antarctida { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(683803510, "antarctida");
        public Entity Arrowsoflove { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-400341658, "arrowsoflove");
        public Entity Autumnleaves { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-662236117, "autumnleaves");
        public Entity Bananas { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-574655849, "bananas");
        public Entity Beginning { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1500327034, "beginning");
        public Entity Birthday2017 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(776075330, "birthday2017");
        public Entity Blackroger { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(2008328619, "blackroger");
        public Entity Blue { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(137747283, "blue");
        public Entity Bluemagma { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(436923060, "bluemagma");
        public Entity Canyon { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(214261055, "canyon");
        public Entity Carbon { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(214261056, "carbon");
        public Entity Ceder { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-24102762, "ceder");
        public Entity Champion { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1798212322, "champion");
        public Entity Coal { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(920905258, "coal");
        public Entity Corrosion { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1569952799, "corrosion");
        public Entity Desert { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-718109490, "desert");
        public Entity Digital { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-683002753, "digital");
        public Entity Dirt { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(137803904, "dirt");
        public Entity Dragon { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-706638132, "dragon");
        public Entity Easternstar { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1717585789, "easternstar");
        public Entity Epicgold { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-892221448, "epicgold");
        public Entity Flame { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1514026752, "flame");
        public Entity Flora { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-21112705, "flora");
        public Entity Football { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(761115110, "football");
        public Entity Forester { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(833674243, "forester");
        public Entity Freedom { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1347685821, "freedom");
        public Entity Frontier1 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1396691349, "frontier1");
        public Entity Frontier2018 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1396691350, "frontier2018");
        public Entity Galaxy { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-636128311, "galaxy");
        public Entity Glacier { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(526265313, "glacier");
        public Entity Glina { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-20195074, "glina");
        public Entity Gold { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(921024755, "gold");
        public Entity Green { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-20020438, "green");
        public Entity Greenskulls { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(97943767, "greenskulls");
        public Entity Halloween { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(721518013, "halloween");
        public Entity Halloween2017 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1218811098, "halloween2017");
        public Entity Hero { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(137919219, "hero");
        public Entity Honeycomb { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1023813172, "honeycomb");
        public Entity Hvoya { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-18967536, "hvoya");
        public Entity Hydra { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-887676103, "hydra");
        public Entity Inferno { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-398350090, "inferno");
        public Entity Leaguebronze { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1185540727, "league_bronze");
        public Entity Leaguegold { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1245546371, "league_gold");
        public Entity Leaguemaster { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-317558887, "league_master");
        public Entity Leaguesilver { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1663842282, "league_silver");
        public Entity Marine { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-464167301, "marine");
        public Entity Mary { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(138064340, "mary");
        public Entity Matteblack { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-646190818, "matteblack");
        public Entity Matteblue { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-436467641, "matteblue");
        public Entity Mattegray { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-431689890, "mattegray");
        public Entity Mattegreen { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-497382758, "mattegreen");
        public Entity Mattenavi { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-425712005, "mattenavi");
        public Entity Matteorange { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(267667571, "matteorange");
        public Entity Mattered { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1260536244, "mattered");
        public Entity Matterose { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-421603854, "matterose");
        public Entity Matteviolet { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1940431938, "matteviolet");
        public Entity Mattewhite { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-48418252, "mattewhite");
        public Entity Matteyellow { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(190698221, "matteyellow");
        public Entity May20181 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(2070833571, "may2018_1");
        public Entity May20182 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(2070833572, "may2018_2");
        public Entity Metel { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-14852182, "metel");
        public Entity Military { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1660396512, "military");
        public Entity Mosaic { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-451216061, "mosaic");
        public Entity Moscow { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1878474135, "moscow");
        public Entity Nefrit { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(532533505, "nefrit");
        public Entity Newyearglow { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1760088841, "newyearglow");
        public Entity Newyearice { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-888059423, "newyearice");
        public Entity Noise { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1506541337, "noise");
        public Entity North { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1506532654, "north");
        public Entity Orange { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-391711001, "orange");
        public Entity Orion { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-12628116, "orion");
        public Entity Partizan { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1555319306, "partizan");
        public Entity Patina { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1079898253, "patina");
        public Entity Pink { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(921287177, "pink");
        public Entity Polar { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1504691957, "polar");
        public Entity Priboy { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-362854786, "priboy");
        public Entity Prodigy { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1641998027, "prodigy");
        public Entity Radiation { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1223441167, "radiation");
        public Entity Red { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(558647928, "red");
        public Entity Redamber { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1784687185, "redamber");
        public Entity Rivets { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(925876519, "rivets");
        public Entity Rocks { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-9952811, "rocks");
        public Entity Rust { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(276536681, "rust");
        public Entity Sandstone { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(138818312, "sandstone");
        public Entity Savanna { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-470773521, "savanna");
        public Entity Smoke { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-9077354, "smoke");
        public Entity Spectre { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-56970637, "spectre");
        public Entity Spiderweb { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(355636982, "spiderweb");
        public Entity Sport { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1207082244, "sport");
        public Entity Starrysky { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1732490966, "starrysky");
        public Entity Steamulatorf { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1880146981, "steamulatorf");
        public Entity Steamulatorsf { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1844971488, "steamulatorsf");
        public Entity Steel { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1501779048, "steel");
        public Entity Stone { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1756632159, "stone");
        public Entity Storm { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-8868592, "storm");
        public Entity Stvalentine2017 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(986557478, "stvalentine2017");
        public Entity Summer { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-274067352, "summer");
        public Entity Swamp { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-8792825, "swamp");
        public Entity Taiga { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-8517219, "taiga");
        public Entity Tar { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1209537658, "tar");
        public Entity Tina { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(138280417, "tina");
        public Entity Trianglecamo { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-1902859834, "trianglecamo");
        public Entity Trollface { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(1546602523, "trollface");
        public Entity Tundra { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-245416673, "tundra");
        public Entity Universe { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(942805628, "universe");
        public Entity Urban { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-7094151, "urban");
        public Entity Vostok1 { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-2023428617, "vostok1");
        public Entity Walker { get; private set; } = TankPaintMarketItemTemplate.CreateEntity(-50932577, "walker");
    }
}
