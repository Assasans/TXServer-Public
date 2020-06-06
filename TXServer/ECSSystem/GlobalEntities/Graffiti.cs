using System.Reflection;
using System.Runtime.Serialization;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Graffiti : ItemList
    {
        public static Graffiti GlobalItems { get; } = new Graffiti();

        public static ItemList GetUserItems(Entity user)
        {
            ItemList items = FormatterServices.GetUninitializedObject(typeof(Graffiti)) as ItemList;

            foreach (PropertyInfo info in typeof(Graffiti).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity Dictator { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(-779102498, "dictator", Hulls.GlobalItems.Dictator);
        public Entity Flamethrower { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(-1319913646, "flamethrower", Weapons.GlobalItems.Flamethrower);
        public Entity Freeze { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(1530725353, "freeze", Weapons.GlobalItems.Freeze);
        public Entity Hammer { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(1590999596, "hammer", Weapons.GlobalItems.Hammer);
        public Entity Hornet { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(1585608388, "hornet", Hulls.GlobalItems.Hornet);
        public Entity Hunter { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(1591036114, "hunter", Hulls.GlobalItems.Hunter);
        public Entity Isis { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(-758092378, "isis", Weapons.GlobalItems.Isis);
        public Entity Mammoth { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(1646325085, "mammoth", Hulls.GlobalItems.Mammoth);
        public Entity Railgun { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(1785144668, "railgun", Weapons.GlobalItems.Railgun);
        public Entity Ricochet { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(2140835849, "ricochet", Weapons.GlobalItems.Ricochet);
        public Entity Shaft { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(-2017127704, "shaft", Weapons.GlobalItems.Shaft);
        public Entity Smoky { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(-2016965135, "smoky", Weapons.GlobalItems.Smoky);
        public Entity Thunder { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(-523272750, "thunder", Weapons.GlobalItems.Thunder);
        public Entity Titan { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(-2016156294, "titan", Hulls.GlobalItems.Titan);
        public Entity Twins { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(-2015749383, "twins", Weapons.GlobalItems.Twins);
        public Entity Viking { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(1980662300, "viking", Hulls.GlobalItems.Viking);
        public Entity Vulcan { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(1991768181, "vulcan", Weapons.GlobalItems.Vulcan);
        public Entity Wasp { get; private set; } = ChildGraffitiMarketItemTemplate.CreateEntity(-757692295, "wasp", Hulls.GlobalItems.Wasp);
        public Entity _23february20171 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-571325446, "23february2017_1");
        public Entity _23february20172 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-571325445, "23february2017_2");
        public Entity _23february2018 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(473147977, "23february2018");
        public Entity _8marchcat { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1669172271, "8marchcat");
        public Entity _8marchcrystal { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(2069414001, "8marchcrystal");
        public Entity Alien { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-2022929356, "alien");
        public Entity Antaeus { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1628984953, "antaeus");
        public Entity Armsrace { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1968456698, "armsrace");
        public Entity Atlas { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(968765463, "atlas");
        public Entity Attackoftitan { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(891728345, "attackoftitan");
        public Entity Birthday2017graffiti { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1278226971, "birthday2017graffiti");
        public Entity Bulldog { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-107843627, "bulldog");
        public Entity Bullterier { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(315308508, "bullterier");
        public Entity Caveman { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-24677874, "caveman");
        public Entity Cosmosx { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(678025425, "cosmosx");
        public Entity Demolisher { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-343773791, "demolisher");
        public Entity Demon { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(971090551, "demon");
        public Entity Devilsheart { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-194604214, "devilsheart");
        public Entity Doberman { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(519082359, "doberman");
        public Entity Fetih { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(292147378, "fetih");
        public Entity Frontier { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-781845119, "frontier");
        public Entity Ghosttrain { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1779805396, "ghosttrain");
        public Entity Giant { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1968456699, "giant");
        public Entity Godmode { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(880222526, "godmode");
        public Entity Goldbill { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1737433896, "goldbill");
        public Entity Goldbox { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(887330442, "goldbox");
        public Entity Hellhound { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1951474656, "hellhound");
        public Entity Heroorder { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(113459975, "heroorder");
        public Entity Hydra { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1289970222, "hydra");
        public Entity Iwojima { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-303400441, "iwojima");
        public Entity Letsgo { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1391443781, "letsgo");
        public Entity Logo { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1001404575, "logo");
        public Entity Luck { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-93361699, "luck");
        public Entity Marshalstar { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(988897323, "marshalstar");
        public Entity May20181 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1775068808, "may2018_1");
        public Entity May20182 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1775068809, "may2018_2");
        public Entity Monster { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(895058022, "monster");
        public Entity Motherland { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-2050212779, "motherland");
        public Entity Nuke { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1001470037, "nuke");
        public Entity Partners { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1611647112, "partners");
        public Entity Peace { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(674847565, "peace");
        public Entity Pixelduck { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1384981436, "pixelduck");
        public Entity Pumpkin { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(457993351, "pumpkin");
        public Entity Pumpkin2017 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1786896255, "pumpkin2017");
        public Entity Season0 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-247809917, "season0");
        public Entity Season0top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(545710246, "season0top");
        public Entity Season1 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-247809886, "season1");
        public Entity Season1top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(546633767, "season1top");
        public Entity Season5 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1690446069, "season5");
        public Entity Season5top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1587411008, "season5top");
        public Entity Season6 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1626810943, "season6");
        public Entity Season6top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-86050122, "season6top");
        public Entity Season10 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1834580363, "seasons/season10");
        public Entity Season10top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-524637504, "seasons/season10top");
        public Entity Season11 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1834580367, "seasons/season11");
        public Entity Season11top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-524637507, "seasons/season11top");
        public Entity Season12 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1834580368, "seasons/season12");
        public Entity Season12top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-524637508, "seasons/season12top");
        public Entity Season2 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(217914652, "seasons/season2");
        public Entity Season2top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-2095038791, "seasons/season2top");
        public Entity Season3 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(217914653, "seasons/season3");
        public Entity Season3top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-2095009000, "seasons/season3top");
        public Entity Season3_ny2018 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1994390162, "seasons/season3_ny2018");
        public Entity Season3_ny2018top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1700141693, "seasons/season3_ny2018top");
        public Entity Season4 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(217914654, "seasons/season4");
        public Entity Season4top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-2094979209, "seasons/season4top");
        public Entity Season7 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(345912680, "seasons/season7");
        public Entity Season7top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1792984208, "seasons/season7top");
        public Entity Season8 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(217914658, "seasons/season8");
        public Entity Season8top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-2094860045, "seasons/season8top");
        public Entity Season9 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(217914659, "seasons/season9");
        public Entity Season9top { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-2094830254, "seasons/season9top");
        public Entity Steampunk { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(62042919, "steampunk");
        public Entity Stvalentine20171 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1808985494, "stvalentine2017_1");
        public Entity Stvalentine20172 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1808985493, "stvalentine2017_2");
        public Entity Tanki2 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1086271184, "tanki2");
        public Entity Up { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1884609450, "up");
        public Entity Valhalladrakkar { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1285677037, "valhalla_drakkar");
        public Entity Valhallavalkyrie { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(619790646, "valhalla_valkyrie");
        public Entity Valhallayggdrasil { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1871711547, "valhalla_yggdrasil");
        public Entity Vdnhstatue { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-1333610377, "vdnhstatue");
        public Entity Waroftheworlds { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1374508848, "waroftheworlds");
        public Entity Widow { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1704628950, "widow");
        public Entity Xmas1 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(-25726372, "xmas1");
        public Entity Xmas2 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(432765771, "xmas2");
        public Entity Xmas3 { get; private set; } = GraffitiMarketItemTemplate.CreateEntity(1435022351, "xmas3");
    }
}
