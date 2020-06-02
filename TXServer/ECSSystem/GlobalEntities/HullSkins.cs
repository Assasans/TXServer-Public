using System.Reflection;
using System.Runtime.Serialization;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class HullSkins : ItemList
    {
        public static HullSkins GlobalItems { get; } = new HullSkins();

        public static ItemList GetUserItems(Entity user)
        {
            ItemList items = FormatterServices.GetUninitializedObject(typeof(HullSkins)) as ItemList;

            foreach (PropertyInfo info in typeof(HullSkins).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity DictatorDreadnought { get; private set; } = new Entity(732611697, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/dictator/dreadnought"),
            new ParentGroupComponent(Hulls.GlobalItems.Dictator),
            new MarketItemGroupComponent(732611697));
        public Entity DictatorM0 { get; private set; } = new Entity(-1555682652, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/dictator/m0"),
            new ParentGroupComponent(Hulls.GlobalItems.Dictator),
            new MarketItemGroupComponent(-1555682652),
            new DefaultSkinItemComponent());
        public Entity DictatorM1 { get; private set; } = new Entity(-1555682651, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/dictator/m1"),
            new ParentGroupComponent(Hulls.GlobalItems.Dictator),
            new MarketItemGroupComponent(-1555682651));
        public Entity DictatorM1gold { get; private set; } = new Entity(-767847828, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/dictator/m1gold"),
            new ParentGroupComponent(Hulls.GlobalItems.Dictator),
            new MarketItemGroupComponent(-767847828));
        public Entity DictatorM1steel { get; private set; } = new Entity(-1626035585, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/dictator/m1steel"),
            new ParentGroupComponent(Hulls.GlobalItems.Dictator),
            new MarketItemGroupComponent(-1626035585));
        public Entity DictatorM2 { get; private set; } = new Entity(783713997, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/dictator/m2"),
            new ParentGroupComponent(Hulls.GlobalItems.Dictator),
            new MarketItemGroupComponent(783713997));
        public Entity DictatorM2flame { get; private set; } = new Entity(1994127174, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/dictator/m2flame"),
            new ParentGroupComponent(Hulls.GlobalItems.Dictator),
            new MarketItemGroupComponent(1994127174));
        public Entity HornetCry { get; private set; } = new Entity(1765925144, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/cry"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(1765925144));
        public Entity HornetCrydischarge { get; private set; } = new Entity(-955210933, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/crydischarge"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(-955210933));
        public Entity HornetCryglow { get; private set; } = new Entity(-955210932, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/cryglow"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(-955210932));
        public Entity HornetCryrage { get; private set; } = new Entity(-955210934, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/cryrage"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(-955210934));
        public Entity HornetM0 { get; private set; } = new Entity(-1194388226, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/m0"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(-1194388226),
            new DefaultSkinItemComponent());
        public Entity HornetM1 { get; private set; } = new Entity(-1194388225, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/m1"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(-1194388225));
        public Entity HornetM1gold { get; private set; } = new Entity(679021958, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/m1gold"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(679021958));
        public Entity HornetM1steel { get; private set; } = new Entity(-378883611, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/m1steel"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(-378883611));
        public Entity HornetM2 { get; private set; } = new Entity(-1992749017, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/m2"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(-1992749017));
        public Entity HornetMay2017 { get; private set; } = new Entity(963790407, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/may2017"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(963790407));
        public Entity HornetT0 { get; private set; } = new Entity(-47777666, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/t0"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(-47777666));
        public Entity HornetT0gold { get; private set; } = new Entity(-1475671586, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/t0gold"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(-1475671586));
        public Entity HornetXt { get; private set; } = new Entity(-47777474, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/xt"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(-47777474));
        public Entity HornetXt_thor { get; private set; } = new Entity(-1636606664, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hornet/xt_thor"),
            new ParentGroupComponent(Hulls.GlobalItems.Hornet),
            new MarketItemGroupComponent(-1636606664));
        public Entity Hunter23february2017 { get; private set; } = new Entity(247232443, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hunter/23february2017"),
            new ParentGroupComponent(Hulls.GlobalItems.Hunter),
            new MarketItemGroupComponent(247232443));
        public Entity HunterM0 { get; private set; } = new Entity(1589207088, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hunter/m0"),
            new ParentGroupComponent(Hulls.GlobalItems.Hunter),
            new MarketItemGroupComponent(1589207088),
            new DefaultSkinItemComponent());
        public Entity HunterM1 { get; private set; } = new Entity(1589207089, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hunter/m1"),
            new ParentGroupComponent(Hulls.GlobalItems.Hunter),
            new MarketItemGroupComponent(1589207089));
        public Entity HunterM1gold { get; private set; } = new Entity(-318345288, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hunter/m1gold"),
            new ParentGroupComponent(Hulls.GlobalItems.Hunter),
            new MarketItemGroupComponent(-318345288));
        public Entity HunterM1steel { get; private set; } = new Entity(-1267544717, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hunter/m1steel"),
            new ParentGroupComponent(Hulls.GlobalItems.Hunter),
            new MarketItemGroupComponent(-1267544717));
        public Entity HunterM2 { get; private set; } = new Entity(1589207091, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hunter/m2"),
            new ParentGroupComponent(Hulls.GlobalItems.Hunter),
            new MarketItemGroupComponent(1589207091));
        public Entity HunterMay2017 { get; private set; } = new Entity(110176853, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hunter/may2017"),
            new ParentGroupComponent(Hulls.GlobalItems.Hunter),
            new MarketItemGroupComponent(110176853));
        public Entity HunterXt { get; private set; } = new Entity(790846704, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hunter/xt"),
            new ParentGroupComponent(Hulls.GlobalItems.Hunter),
            new MarketItemGroupComponent(790846704));
        public Entity HunterXt_thor { get; private set; } = new Entity(1804747078, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/hunter/xt_thor"),
            new ParentGroupComponent(Hulls.GlobalItems.Hunter),
            new MarketItemGroupComponent(1804747078));
        public Entity MammothM0 { get; private set; } = new Entity(-543026971, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/mammoth/m0"),
            new ParentGroupComponent(Hulls.GlobalItems.Mammoth),
            new MarketItemGroupComponent(-543026971),
            new DefaultSkinItemComponent());
        public Entity MammothM1 { get; private set; } = new Entity(-543026970, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/mammoth/m1"),
            new ParentGroupComponent(Hulls.GlobalItems.Mammoth),
            new MarketItemGroupComponent(-543026970));
        public Entity MammothM1gold { get; private set; } = new Entity(-920939649, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/mammoth/m1gold"),
            new ParentGroupComponent(Hulls.GlobalItems.Mammoth),
            new MarketItemGroupComponent(-920939649));
        public Entity MammothM1steel { get; private set; } = new Entity(-833500654, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/mammoth/m1steel"),
            new ParentGroupComponent(Hulls.GlobalItems.Mammoth),
            new MarketItemGroupComponent(-833500654));
        public Entity MammothM2 { get; private set; } = new Entity(-543026935, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/mammoth/m2"),
            new ParentGroupComponent(Hulls.GlobalItems.Mammoth),
            new MarketItemGroupComponent(-543026935));
        public Entity MammothSteam { get; private set; } = new Entity(677046075, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/mammoth/steam"),
            new ParentGroupComponent(Hulls.GlobalItems.Mammoth),
            new MarketItemGroupComponent(677046075));
        public Entity TitanFrontierzero { get; private set; } = new Entity(1470436461, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/titan/frontierzero"),
            new ParentGroupComponent(Hulls.GlobalItems.Titan),
            new MarketItemGroupComponent(1470436461));
        public Entity TitanM0 { get; private set; } = new Entity(-1584239704, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/titan/m0"),
            new ParentGroupComponent(Hulls.GlobalItems.Titan),
            new MarketItemGroupComponent(-1584239704),
            new DefaultSkinItemComponent());
        public Entity TitanM1 { get; private set; } = new Entity(-1584239703, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/titan/m1"),
            new ParentGroupComponent(Hulls.GlobalItems.Titan),
            new MarketItemGroupComponent(-1584239703));
        public Entity TitanM1gold { get; private set; } = new Entity(-754659582, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/titan/m1gold"),
            new ParentGroupComponent(Hulls.GlobalItems.Titan),
            new MarketItemGroupComponent(-754659582));
        public Entity TitanM1steel { get; private set; } = new Entity(-133498961, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/titan/m1steel"),
            new ParentGroupComponent(Hulls.GlobalItems.Titan),
            new MarketItemGroupComponent(-133498961));
        public Entity TitanM2 { get; private set; } = new Entity(-1584239766, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/titan/m2"),
            new ParentGroupComponent(Hulls.GlobalItems.Titan),
            new MarketItemGroupComponent(-1584239766));
        public Entity TitanM2tsk { get; private set; } = new Entity(-651639290, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/titan/m2tsk"),
            new ParentGroupComponent(Hulls.GlobalItems.Titan),
            new MarketItemGroupComponent(-651639290));
        public Entity TitanXt { get; private set; } = new Entity(1576595770, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/titan/xt"),
            new ParentGroupComponent(Hulls.GlobalItems.Titan),
            new MarketItemGroupComponent(1576595770));
        public Entity TitanXt_thor { get; private set; } = new Entity(1163905852, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/titan/xt_thor"),
            new ParentGroupComponent(Hulls.GlobalItems.Titan),
            new MarketItemGroupComponent(1163905852));
        public Entity VikingCry { get; private set; } = new Entity(-1353746752, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/cry"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(-1353746752));
        public Entity VikingCrydischarge { get; private set; } = new Entity(-1363672965, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/crydischarge"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(-1363672965));
        public Entity VikingCryglow { get; private set; } = new Entity(-110402718, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/cryglow"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(-110402718));
        public Entity VikingCryrage { get; private set; } = new Entity(-110775867, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/cryrage"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(-110775867));
        public Entity VikingM0 { get; private set; } = new Entity(-353686874, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/m0"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(-353686874),
            new DefaultSkinItemComponent());
        public Entity VikingM1 { get; private set; } = new Entity(-353686873, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/m1"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(-353686873));
        public Entity VikingM1gold { get; private set; } = new Entity(499257134, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/m1gold"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(499257134));
        public Entity VikingM1steel { get; private set; } = new Entity(-1656625859, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/m1steel"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(-1656625859));
        public Entity VikingM2 { get; private set; } = new Entity(792923471, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/m2"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(792923471));
        public Entity VikingMay2017 { get; private set; } = new Entity(-313951841, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/may2017"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(-313951841));
        public Entity VikingT2 { get; private set; } = new Entity(-1152047448, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/t2"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(-1152047448));
        public Entity VikingXt { get; private set; } = new Entity(792923878, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/xt"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(792923878));
        public Entity VikingXt_thor { get; private set; } = new Entity(1380618384, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/viking/xt_thor"),
            new ParentGroupComponent(Hulls.GlobalItems.Viking),
            new MarketItemGroupComponent(1380618384));
        public Entity Wasp8march2017 { get; private set; } = new Entity(-1648311780, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/wasp/8march2017"),
            new ParentGroupComponent(Hulls.GlobalItems.Wasp),
            new MarketItemGroupComponent(-1648311780));
        public Entity WaspDreadnought { get; private set; } = new Entity(-2009337684, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/wasp/Dreadnought"),
            new ParentGroupComponent(Hulls.GlobalItems.Wasp),
            new MarketItemGroupComponent(-2009337684));
        public Entity WaspM0 { get; private set; } = new Entity(-1774581975, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/wasp/m0"),
            new ParentGroupComponent(Hulls.GlobalItems.Wasp),
            new MarketItemGroupComponent(-1774581975),
            new DefaultSkinItemComponent());
        public Entity WaspM1 { get; private set; } = new Entity(-1774581974, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/wasp/m1"),
            new ParentGroupComponent(Hulls.GlobalItems.Wasp),
            new MarketItemGroupComponent(-1774581974));
        public Entity WaspM1gold { get; private set; } = new Entity(-1774441975, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/wasp/m1gold"),
            new ParentGroupComponent(Hulls.GlobalItems.Wasp),
            new MarketItemGroupComponent(-1774441975));
        public Entity WaspM1steel { get; private set; } = new Entity(-1555805158, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/wasp/m1steel"),
            new ParentGroupComponent(Hulls.GlobalItems.Wasp),
            new MarketItemGroupComponent(-1555805158));
        public Entity WaspM2 { get; private set; } = new Entity(-10051566, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/wasp/m2"),
            new ParentGroupComponent(Hulls.GlobalItems.Wasp),
            new MarketItemGroupComponent(-10051566));
        public Entity WaspXmas { get; private set; } = new Entity(72041592, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/wasp/xmas"),
            new ParentGroupComponent(Hulls.GlobalItems.Wasp),
            new MarketItemGroupComponent(72041592));
        public Entity WaspXt { get; private set; } = new Entity(-1129770519, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/wasp/xt"),
            new ParentGroupComponent(Hulls.GlobalItems.Wasp),
            new MarketItemGroupComponent(-1129770519));
        public Entity WaspXt_thor { get; private set; } = new Entity(-1838544851, new TemplateAccessor(new HullSkinMarketItemTemplate(), "garage/skin/tank/wasp/xt_thor"),
            new ParentGroupComponent(Hulls.GlobalItems.Wasp),
            new MarketItemGroupComponent(-1838544851));
    }
}
