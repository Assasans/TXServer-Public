using System.Reflection;
using System.Runtime.Serialization;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Covers : ItemList
    {
        public static Covers GlobalItems { get; } = new Covers();

        public static ItemList GetUserItems(Entity user)
        {
            ItemList items = FormatterServices.GetUninitializedObject(typeof(Covers)) as ItemList;

            foreach (PropertyInfo info in typeof(Covers).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity Antaeus1 { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-891349912, "antaeus1");
        public Entity Antaeus2018 { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1577640270, "antaeus2018");
        public Entity Blue { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-172609771, "blue");
        public Entity Carbon { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1644116328, "carbon");
        public Entity Ceder { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1055236844, "ceder");
        public Entity Champion { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(888876452, "champion");
        public Entity Coal { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-172577710, "coal");
        public Entity Corrosion { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-849655395, "corrosion");
        public Entity Desert { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1676472336, "desert");
        public Entity Digital { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(534589821, "digital");
        public Entity Dirt { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-172553150, "dirt");
        public Entity Dragon { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1687943694, "dragon");
        public Entity Flame { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1052260392, "flame");
        public Entity Flora { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1052246787, "flora");
        public Entity Forester { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-75661627, "forester");
        public Entity Frontier1 { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(610004612, "frontier1");
        public Entity Frontier2018 { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(640844210, "frontier2018");
        public Entity Galaxy { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-64859568, "galaxy");
        public Entity Glacier { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1705840017, "glacier");
        public Entity Glina { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1051329156, "glina");
        public Entity Gold { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-172458213, "gold");
        public Entity Green { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1051154520, "green");
        public Entity Greenskulls { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1953107513, "greenskulls");
        public Entity Halloween { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1128248718, "halloween");
        public Entity Honeycomb { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-784118404, "honeycomb");
        public Entity Hvoya { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1050101618, "hvoya");
        public Entity Inferno { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(819242484, "inferno");
        public Entity Izumrud { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(372065215, "izumrud");
        public Entity Lightblue { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1167717168, "lightblue");
        public Entity Marine { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1930414525, "marine");
        public Entity Mary { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-172292714, "mary");
        public Entity Matteblack { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1958622537, "matteblack");
        public Entity Matteblue { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1448655312, "matteblue");
        public Entity Mattegray { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1448809433, "mattegray");
        public Entity Mattegreen { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1963422797, "mattegreen");
        public Entity Mattenavi { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1449002268, "mattenavi");
        public Entity Matteorange { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(965487140, "matteorange");
        public Entity Mattered { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-230348709, "mattered");
        public Entity Matterose { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1449134789, "matterose");
        public Entity Matteviolet { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1157994613, "matteviolet");
        public Entity Mattewhite { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1977905523, "mattewhite");
        public Entity Matteyellow { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1240098922, "matteyellow");
        public Entity May20181 { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-217437764, "may2018_1");
        public Entity May20182 { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-217437763, "may2018_2");
        public Entity Metel { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1045986264, "metel");
        public Entity Nefrit { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1962388777, "nefrit");
        public Entity Newyearglow { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1273539023, "newyearglow");
        public Entity Newyearice { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-97463799, "newyearice");
        public Entity Noise { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1044774977, "noise");
        public Entity None { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-172249613, "none");
        public Entity North { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1044766294, "north");
        public Entity Orange { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(2002870825, "orange");
        public Entity Partizan { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(645983436, "partizan");
        public Entity Patina { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(2016361556, "patina");
        public Entity Pink { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-172195791, "pink");
        public Entity Polar { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1042925597, "polar");
        public Entity Priboy { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(2031727040, "priboy");
        public Entity Prodigy { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1435376695, "prodigy");
        public Entity Radiation { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-500584127, "radiation");
        public Entity Red { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(410089078, "red");
        public Entity Rivets { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(2081063968, "rivets");
        public Entity Rocks { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1041086893, "rocks");
        public Entity Rust { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-172124513, "rust");
        public Entity Sandstone { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(2014177414, "sandstone");
        public Entity Savanna { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(746819053, "savanna");
        public Entity Smoke { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1040211436, "smoke");
        public Entity Sport { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1040121831, "sport");
        public Entity Starrysky { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1779740531, "starrysky");
        public Entity Steamulatorf { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(1408168452, "steamulatorf");
        public Entity Steamulatorsf { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(703549557, "steamulatorsf");
        public Entity Steel { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1040012688, "steel");
        public Entity Stone { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1040002806, "stone");
        public Entity Storm { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1040002674, "storm");
        public Entity Stvalentine2017 { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-26936066, "stvalentine2017");
        public Entity Swamp { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1039926907, "swamp");
        public Entity Taiga { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1039651301, "taiga");
        public Entity Tar { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(410090890, "tar");
        public Entity Tina { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-172076637, "tina");
        public Entity Trianglecamo { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-2097840184, "trianglecamo");
        public Entity Tundra { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-2145802143, "tundra");
        public Entity Universe { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(176253763, "universe");
        public Entity Urban { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-1038228233, "urban");
        public Entity Vostok1 { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-487032800, "vostok1");
        public Entity Walker { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(-2078438351, "walker");
        public Entity Yellow { get; private set; } = WeaponPaintMarketItemTemplate.CreateEntity(454169524, "yellow");
    }
}
