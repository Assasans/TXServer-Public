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

        public Entity DictatorDreadnought { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(732611697, "dictator/dreadnought", Hulls.GlobalItems.Dictator);
        public Entity DictatorM0 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1555682652, "dictator/m0", Hulls.GlobalItems.Dictator, isDefault: true);
        public Entity DictatorM1 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1555682651, "dictator/m1", Hulls.GlobalItems.Dictator);
        public Entity DictatorM1gold { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-767847828, "dictator/m1gold", Hulls.GlobalItems.Dictator);
        public Entity DictatorM1steel { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1626035585, "dictator/m1steel", Hulls.GlobalItems.Dictator);
        public Entity DictatorM2 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(783713997, "dictator/m2", Hulls.GlobalItems.Dictator);
        public Entity DictatorM2flame { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(1994127174, "dictator/m2flame", Hulls.GlobalItems.Dictator);
        public Entity HornetCry { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(1765925144, "hornet/cry", Hulls.GlobalItems.Hornet);
        public Entity HornetCrydischarge { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-955210933, "hornet/crydischarge", Hulls.GlobalItems.Hornet);
        public Entity HornetCryglow { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-955210932, "hornet/cryglow", Hulls.GlobalItems.Hornet);
        public Entity HornetCryrage { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-955210934, "hornet/cryrage", Hulls.GlobalItems.Hornet);
        public Entity HornetM0 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1194388226, "hornet/m0", Hulls.GlobalItems.Hornet, isDefault: true);
        public Entity HornetM1 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1194388225, "hornet/m1", Hulls.GlobalItems.Hornet);
        public Entity HornetM1gold { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(679021958, "hornet/m1gold", Hulls.GlobalItems.Hornet);
        public Entity HornetM1steel { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-378883611, "hornet/m1steel", Hulls.GlobalItems.Hornet);
        public Entity HornetM2 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1992749017, "hornet/m2", Hulls.GlobalItems.Hornet);
        public Entity HornetMay2017 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(963790407, "hornet/may2017", Hulls.GlobalItems.Hornet);
        public Entity HornetT0 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-47777666, "hornet/t0", Hulls.GlobalItems.Hornet);
        public Entity HornetT0gold { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1475671586, "hornet/t0gold", Hulls.GlobalItems.Hornet);
        public Entity HornetXt { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-47777474, "hornet/xt", Hulls.GlobalItems.Hornet);
        public Entity HornetXtthor { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1636606664, "hornet/xt_thor", Hulls.GlobalItems.Hornet);
        public Entity Hunter23february2017 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(247232443, "hunter/23february2017", Hulls.GlobalItems.Hunter);
        public Entity HunterM0 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(1589207088, "hunter/m0", Hulls.GlobalItems.Hunter, isDefault: true);
        public Entity HunterM1 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(1589207089, "hunter/m1", Hulls.GlobalItems.Hunter);
        public Entity HunterM1gold { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-318345288, "hunter/m1gold", Hulls.GlobalItems.Hunter);
        public Entity HunterM1steel { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1267544717, "hunter/m1steel", Hulls.GlobalItems.Hunter);
        public Entity HunterM2 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(1589207091, "hunter/m2", Hulls.GlobalItems.Hunter);
        public Entity HunterMay2017 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(110176853, "hunter/may2017", Hulls.GlobalItems.Hunter);
        public Entity HunterXt { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(790846704, "hunter/xt", Hulls.GlobalItems.Hunter);
        public Entity HunterXtthor { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(1804747078, "hunter/xt_thor", Hulls.GlobalItems.Hunter);
        public Entity MammothM0 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-543026971, "mammoth/m0", Hulls.GlobalItems.Mammoth, isDefault: true);
        public Entity MammothM1 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-543026970, "mammoth/m1", Hulls.GlobalItems.Mammoth);
        public Entity MammothM1gold { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-920939649, "mammoth/m1gold", Hulls.GlobalItems.Mammoth);
        public Entity MammothM1steel { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-833500654, "mammoth/m1steel", Hulls.GlobalItems.Mammoth);
        public Entity MammothM2 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-543026935, "mammoth/m2", Hulls.GlobalItems.Mammoth);
        public Entity MammothSteam { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(677046075, "mammoth/steam", Hulls.GlobalItems.Mammoth);
        public Entity TitanFrontierzero { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(1470436461, "titan/frontierzero", Hulls.GlobalItems.Titan);
        public Entity TitanM0 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1584239704, "titan/m0", Hulls.GlobalItems.Titan, isDefault: true);
        public Entity TitanM1 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1584239703, "titan/m1", Hulls.GlobalItems.Titan);
        public Entity TitanM1gold { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-754659582, "titan/m1gold", Hulls.GlobalItems.Titan);
        public Entity TitanM1steel { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-133498961, "titan/m1steel", Hulls.GlobalItems.Titan);
        public Entity TitanM2 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1584239766, "titan/m2", Hulls.GlobalItems.Titan);
        public Entity TitanM2tsk { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-651639290, "titan/m2tsk", Hulls.GlobalItems.Titan);
        public Entity TitanXt { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(1576595770, "titan/xt", Hulls.GlobalItems.Titan);
        public Entity TitanXtthor { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(1163905852, "titan/xt_thor", Hulls.GlobalItems.Titan);
        public Entity VikingCry { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1353746752, "viking/cry", Hulls.GlobalItems.Viking);
        public Entity VikingCrydischarge { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1363672965, "viking/crydischarge", Hulls.GlobalItems.Viking);
        public Entity VikingCryglow { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-110402718, "viking/cryglow", Hulls.GlobalItems.Viking);
        public Entity VikingCryrage { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-110775867, "viking/cryrage", Hulls.GlobalItems.Viking);
        public Entity VikingM0 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-353686874, "viking/m0", Hulls.GlobalItems.Viking, isDefault: true);
        public Entity VikingM1 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-353686873, "viking/m1", Hulls.GlobalItems.Viking);
        public Entity VikingM1gold { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(499257134, "viking/m1gold", Hulls.GlobalItems.Viking);
        public Entity VikingM1steel { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1656625859, "viking/m1steel", Hulls.GlobalItems.Viking);
        public Entity VikingM2 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(792923471, "viking/m2", Hulls.GlobalItems.Viking);
        public Entity VikingMay2017 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-313951841, "viking/may2017", Hulls.GlobalItems.Viking);
        public Entity VikingT2 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1152047448, "viking/t2", Hulls.GlobalItems.Viking);
        public Entity VikingXt { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(792923878, "viking/xt", Hulls.GlobalItems.Viking);
        public Entity VikingXtthor { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(1380618384, "viking/xt_thor", Hulls.GlobalItems.Viking);
        public Entity Wasp8march2017 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1648311780, "wasp/8march2017", Hulls.GlobalItems.Wasp);
        public Entity WaspDreadnought { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-2009337684, "wasp/Dreadnought", Hulls.GlobalItems.Wasp);
        public Entity WaspM0 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1774581975, "wasp/m0", Hulls.GlobalItems.Wasp, isDefault: true);
        public Entity WaspM1 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1774581974, "wasp/m1", Hulls.GlobalItems.Wasp);
        public Entity WaspM1gold { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1774441975, "wasp/m1gold", Hulls.GlobalItems.Wasp);
        public Entity WaspM1steel { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1555805158, "wasp/m1steel", Hulls.GlobalItems.Wasp);
        public Entity WaspM2 { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-10051566, "wasp/m2", Hulls.GlobalItems.Wasp);
        public Entity WaspXmas { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(72041592, "wasp/xmas", Hulls.GlobalItems.Wasp);
        public Entity WaspXt { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1129770519, "wasp/xt", Hulls.GlobalItems.Wasp);
        public Entity WaspXtthor { get; private set; } = HullSkinMarketItemTemplate.CreateEntity(-1838544851, "wasp/xt_thor", Hulls.GlobalItems.Wasp);
    }
}
