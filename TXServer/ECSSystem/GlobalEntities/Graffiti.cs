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

        public Entity Dictator { get; private set; } = new Entity(-779102498, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/dictator"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Hulls.GlobalItems.Dictator),
            new MarketItemGroupComponent(-779102498));
        public Entity Flamethrower { get; private set; } = new Entity(-1319913646, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/flamethrower"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Weapons.GlobalItems.Flamethrower),
            new MarketItemGroupComponent(-1319913646));
        public Entity Freeze { get; private set; } = new Entity(1530725353, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/freeze"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Weapons.GlobalItems.Freeze),
            new MarketItemGroupComponent(1530725353));
        public Entity Hammer { get; private set; } = new Entity(1590999596, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/hammer"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Weapons.GlobalItems.Hammer),
            new MarketItemGroupComponent(1590999596));
        public Entity Hornet { get; private set; } = new Entity(1585608388, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/hornet"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(1585608388));
        public Entity Hunter { get; private set; } = new Entity(1591036114, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/hunter"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Hulls.GlobalItems.Hunter),
            new MarketItemGroupComponent(1591036114));
        public Entity Isis { get; private set; } = new Entity(-758092378, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/isis"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Weapons.GlobalItems.Isis),
            new MarketItemGroupComponent(-758092378));
        public Entity Mammoth { get; private set; } = new Entity(1646325085, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/mammoth"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Hulls.GlobalItems.Mammoth),
            new MarketItemGroupComponent(1646325085));
        public Entity Railgun { get; private set; } = new Entity(1785144668, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/railgun"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Weapons.GlobalItems.Railgun),
            new MarketItemGroupComponent(1785144668));
        public Entity Ricochet { get; private set; } = new Entity(2140835849, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/ricochet"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Weapons.GlobalItems.Ricochet),
            new MarketItemGroupComponent(2140835849));
        public Entity Shaft { get; private set; } = new Entity(-2017127704, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/shaft"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Weapons.GlobalItems.Shaft),
            new MarketItemGroupComponent(-2017127704));
        public Entity Smoky { get; private set; } = new Entity(-2016965135, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/smoky"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Weapons.GlobalItems.Smoky),
            new MarketItemGroupComponent(-2016965135));
        public Entity Thunder { get; private set; } = new Entity(-523272750, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/thunder"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Weapons.GlobalItems.Thunder),
            new MarketItemGroupComponent(-523272750));
        public Entity Titan { get; private set; } = new Entity(-2016156294, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/titan"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Hulls.GlobalItems.Titan),
            new MarketItemGroupComponent(-2016156294));
        public Entity Twins { get; private set; } = new Entity(-2015749383, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/twins"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Weapons.GlobalItems.Twins),
            new MarketItemGroupComponent(-2015749383));
        public Entity Viking { get; private set; } = new Entity(1980662300, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/viking"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(1980662300));
        public Entity Vulcan { get; private set; } = new Entity(1991768181, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/vulcan"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Weapons.GlobalItems.Vulcan),
            new MarketItemGroupComponent(1991768181));
        public Entity Wasp { get; private set; } = new Entity(-757692295, new TemplateAccessor(new ChildGraffitiMarketItemTemplate(), "garage/graffiti/child/wasp"),
            new RestrictionByUserFractionComponent(),
            new ParentGroupComponent(Hulls.GlobalItems.Wasp),
            new MarketItemGroupComponent(-757692295));
        public Entity _23february2017_1 { get; private set; } = new Entity(-571325446, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/23february2017_1"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-571325446));
        public Entity _23february2017_2 { get; private set; } = new Entity(-571325445, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/23february2017_2"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-571325445));
        public Entity _23february2018 { get; private set; } = new Entity(473147977, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/23february2018"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(473147977));
        public Entity _8marchcat { get; private set; } = new Entity(-1669172271, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/8marchcat"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1669172271));
        public Entity _8marchcrystal { get; private set; } = new Entity(2069414001, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/8marchcrystal"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(2069414001));
        public Entity Alien { get; private set; } = new Entity(-2022929356, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/alien"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-2022929356));
        public Entity Antaeus { get; private set; } = new Entity(-1628984953, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/antaeus"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1628984953));
        public Entity Armsrace { get; private set; } = new Entity(-1968456698, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/armsrace"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1968456698));
        public Entity Atlas { get; private set; } = new Entity(968765463, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/atlas"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(968765463));
        public Entity Attackoftitan { get; private set; } = new Entity(891728345, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/attackoftitan"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(891728345));
        public Entity Birthday2017graffiti { get; private set; } = new Entity(-1278226971, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/birthday2017graffiti"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1278226971));
        public Entity Bulldog { get; private set; } = new Entity(-107843627, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/bulldog"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-107843627));
        public Entity Bullterier { get; private set; } = new Entity(315308508, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/bullterier"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(315308508));
        public Entity Caveman { get; private set; } = new Entity(-24677874, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/caveman"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-24677874));
        public Entity Cosmosx { get; private set; } = new Entity(678025425, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/cosmosx"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(678025425));
        public Entity Demolisher { get; private set; } = new Entity(-343773791, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/demolisher"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-343773791));
        public Entity Demon { get; private set; } = new Entity(971090551, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/demon"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(971090551));
        public Entity Devilsheart { get; private set; } = new Entity(-194604214, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/devilsheart"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-194604214));
        public Entity Doberman { get; private set; } = new Entity(519082359, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/doberman"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(519082359));
        public Entity Fetih { get; private set; } = new Entity(292147378, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/fetih"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(292147378));
        public Entity Frontier { get; private set; } = new Entity(-781845119, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/frontier"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-781845119));
        public Entity Ghosttrain { get; private set; } = new Entity(-1779805396, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/ghosttrain"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1779805396));
        public Entity Giant { get; private set; } = new Entity(-1968456699, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/giant"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1968456699));
        public Entity Godmode { get; private set; } = new Entity(880222526, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/godmode"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(880222526));
        public Entity Goldbill { get; private set; } = new Entity(1737433896, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/goldbill"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1737433896));
        public Entity Goldbox { get; private set; } = new Entity(887330442, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/goldbox"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(887330442));
        public Entity Hellhound { get; private set; } = new Entity(1951474656, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/hellhound"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1951474656));
        public Entity Heroorder { get; private set; } = new Entity(113459975, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/heroorder"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(113459975));
        public Entity Hydra { get; private set; } = new Entity(1289970222, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/hydra"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1289970222));
        public Entity Iwojima { get; private set; } = new Entity(-303400441, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/iwojima"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-303400441));
        public Entity Letsgo { get; private set; } = new Entity(1391443781, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/letsgo"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1391443781));
        public Entity Logo { get; private set; } = new Entity(1001404575, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/logo"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1001404575));
        public Entity Luck { get; private set; } = new Entity(-93361699, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/luck"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-93361699));
        public Entity Marshalstar { get; private set; } = new Entity(988897323, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/marshalstar"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(988897323));
        public Entity May2018_1 { get; private set; } = new Entity(1775068808, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/may2018_1"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1775068808));
        public Entity May2018_2 { get; private set; } = new Entity(1775068809, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/may2018_2"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1775068809));
        public Entity Monster { get; private set; } = new Entity(895058022, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/monster"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(895058022));
        public Entity Motherland { get; private set; } = new Entity(-2050212779, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/motherland"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-2050212779));
        public Entity Nuke { get; private set; } = new Entity(1001470037, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/nuke"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1001470037));
        public Entity Partners { get; private set; } = new Entity(-1611647112, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/partners"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1611647112));
        public Entity Peace { get; private set; } = new Entity(674847565, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/peace"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(674847565));
        public Entity Pixelduck { get; private set; } = new Entity(1384981436, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/pixelduck"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1384981436));
        public Entity Pumpkin { get; private set; } = new Entity(457993351, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/pumpkin"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(457993351));
        public Entity Pumpkin2017 { get; private set; } = new Entity(-1786896255, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/pumpkin2017"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1786896255));
        public Entity Season0 { get; private set; } = new Entity(-247809917, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/season0"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-247809917));
        public Entity Season0top { get; private set; } = new Entity(545710246, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/season0top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(545710246));
        public Entity Season1 { get; private set; } = new Entity(-247809886, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/season1"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-247809886));
        public Entity Season1top { get; private set; } = new Entity(546633767, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/season1top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(546633767));
        public Entity Season5 { get; private set; } = new Entity(1690446069, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/season5"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1690446069));
        public Entity Season5top { get; private set; } = new Entity(1587411008, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/season5top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1587411008));
        public Entity Season6 { get; private set; } = new Entity(1626810943, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/season6"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1626810943));
        public Entity Season6top { get; private set; } = new Entity(-86050122, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/season6top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-86050122));
        public Entity Season10 { get; private set; } = new Entity(-1834580363, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season10"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1834580363));
        public Entity Season10top { get; private set; } = new Entity(-524637504, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season10top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-524637504));
        public Entity Season11 { get; private set; } = new Entity(-1834580367, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season11"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1834580367));
        public Entity Season11top { get; private set; } = new Entity(-524637507, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season11top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-524637507));
        public Entity Season12 { get; private set; } = new Entity(-1834580368, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season12"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1834580368));
        public Entity Season12top { get; private set; } = new Entity(-524637508, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season12top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-524637508));
        public Entity Season2 { get; private set; } = new Entity(217914652, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season2"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(217914652));
        public Entity Season2top { get; private set; } = new Entity(-2095038791, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season2top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-2095038791));
        public Entity Season3 { get; private set; } = new Entity(217914653, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season3"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(217914653));
        public Entity Season3top { get; private set; } = new Entity(-2095009000, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season3top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-2095009000));
        public Entity Season3_ny2018 { get; private set; } = new Entity(1994390162, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season3_ny2018"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1994390162));
        public Entity Season3_ny2018top { get; private set; } = new Entity(-1700141693, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season3_ny2018top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1700141693));
        public Entity Season4 { get; private set; } = new Entity(217914654, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season4"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(217914654));
        public Entity Season4top { get; private set; } = new Entity(-2094979209, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season4top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-2094979209));
        public Entity Season7 { get; private set; } = new Entity(345912680, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season7"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(345912680));
        public Entity Season7top { get; private set; } = new Entity(-1792984208, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season7top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1792984208));
        public Entity Season8 { get; private set; } = new Entity(217914658, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season8"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(217914658));
        public Entity Season8top { get; private set; } = new Entity(-2094860045, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season8top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-2094860045));
        public Entity Season9 { get; private set; } = new Entity(217914659, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season9"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(217914659));
        public Entity Season9top { get; private set; } = new Entity(-2094830254, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/seasons/season9top"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-2094830254));
        public Entity Steampunk { get; private set; } = new Entity(62042919, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/steampunk"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(62042919));
        public Entity Stvalentine2017_1 { get; private set; } = new Entity(-1808985494, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/stvalentine2017_1"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1808985494));
        public Entity Stvalentine2017_2 { get; private set; } = new Entity(-1808985493, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/stvalentine2017_2"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1808985493));
        public Entity Tanki2 { get; private set; } = new Entity(1086271184, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/tanki2"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1086271184));
        public Entity Up { get; private set; } = new Entity(1884609450, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/up"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1884609450));
        public Entity Valhalla_drakkar { get; private set; } = new Entity(1285677037, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/valhalla_drakkar"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1285677037));
        public Entity Valhalla_valkyrie { get; private set; } = new Entity(619790646, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/valhalla_valkyrie"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(619790646));
        public Entity Valhalla_yggdrasil { get; private set; } = new Entity(-1871711547, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/valhalla_yggdrasil"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1871711547));
        public Entity Vdnhstatue { get; private set; } = new Entity(-1333610377, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/vdnhstatue"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-1333610377));
        public Entity Waroftheworlds { get; private set; } = new Entity(1374508848, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/waroftheworlds"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1374508848));
        public Entity Widow { get; private set; } = new Entity(1704628950, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/widow"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1704628950));
        public Entity Xmas1 { get; private set; } = new Entity(-25726372, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/xmas1"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(-25726372));
        public Entity Xmas2 { get; private set; } = new Entity(432765771, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/xmas2"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(432765771));
        public Entity Xmas3 { get; private set; } = new Entity(1435022351, new TemplateAccessor(new GraffitiMarketItemTemplate(), "garage/graffiti/independent/xmas3"),
            new RestrictionByUserFractionComponent(),
            new MarketItemGroupComponent(1435022351));
    }
}
