using System.Reflection;
using System.Runtime.Serialization;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class WeaponSkins : ItemList
    {
        public static WeaponSkins GlobalItems { get; } = new WeaponSkins();

        public static WeaponSkins GetUserItems(Entity user)
        {
            WeaponSkins items = FormatterServices.GetUninitializedObject(typeof(WeaponSkins)) as WeaponSkins;

            foreach (PropertyInfo info in typeof(WeaponSkins).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity FlamethrowerDreadnought { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(602602131, "flamethrower/Dreadnought", Weapons.GlobalItems.Flamethrower);
        public Entity FlamethrowerM0 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(2078566312, "flamethrower/m0", Weapons.GlobalItems.Flamethrower, isDefault: true);
        public Entity FlamethrowerM1 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(2078566313, "flamethrower/m1", Weapons.GlobalItems.Flamethrower);
        public Entity FlamethrowerM1gold { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1491390890, "flamethrower/m1gold", Weapons.GlobalItems.Flamethrower);
        public Entity FlamethrowerM1steel { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-1000298047, "flamethrower/m1steel", Weapons.GlobalItems.Flamethrower);
        public Entity FlamethrowerM2 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1045726795, "flamethrower/m2", Weapons.GlobalItems.Flamethrower);
        public Entity FlamethrowerM2flame { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-983916600, "flamethrower/m2flame", Weapons.GlobalItems.Flamethrower);
        public Entity FreezeM0 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-472765007, "freeze/m0", Weapons.GlobalItems.Freeze, isDefault: true);
        public Entity FreezeM1 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-472765006, "freeze/m1", Weapons.GlobalItems.Freeze);
        public Entity FreezeM1gold { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-362162189, "freeze/m1gold", Weapons.GlobalItems.Freeze);
        public Entity FreezeM1steel { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1669098648, "freeze/m1steel", Weapons.GlobalItems.Freeze);
        public Entity FreezeM2 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(237598868, "freeze/m2", Weapons.GlobalItems.Freeze);
        public Entity FreezeXmas { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(139596834, "freeze/xmas", Weapons.GlobalItems.Freeze);
        public Entity HammerCry { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(283356827, "hammer/cry", Weapons.GlobalItems.Hammer);
        public Entity HammerCrydischarge { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-350323544, "hammer/crydischarge", Weapons.GlobalItems.Hammer);
        public Entity HammerCryglow { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-350323543, "hammer/cryglow", Weapons.GlobalItems.Hammer);
        public Entity HammerCryrage { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-350323545, "hammer/cryrage", Weapons.GlobalItems.Hammer);
        public Entity HammerM0 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-635589854, "hammer/m0", Weapons.GlobalItems.Hammer, isDefault: true);
        public Entity HammerM1 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-635589853, "hammer/m1", Weapons.GlobalItems.Hammer);
        public Entity HammerM1gold { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-1427688220, "hammer/m1gold", Weapons.GlobalItems.Hammer);
        public Entity HammerM1steel { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(2039676487, "hammer/m1steel", Weapons.GlobalItems.Hammer);
        public Entity HammerM2 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1076729477, "hammer/m2", Weapons.GlobalItems.Hammer);
        public Entity Isis8march2017 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-305773183, "isis/8march2017", Weapons.GlobalItems.Isis);
        public Entity IsisM0 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-746649388, "isis/m0", Weapons.GlobalItems.Isis, isDefault: true);
        public Entity IsisM1 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-746649387, "isis/m1", Weapons.GlobalItems.Isis);
        public Entity IsisM1gold { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-1704481322, "isis/m1gold", Weapons.GlobalItems.Isis);
        public Entity IsisM1steel { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-691629355, "isis/m1steel", Weapons.GlobalItems.Isis);
        public Entity IsisM2 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(765745271, "isis/m2", Weapons.GlobalItems.Isis);
        public Entity RailgunM0 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(599453582, "railgun/m0", Weapons.GlobalItems.Railgun, isDefault: true);
        public Entity RailgunM1 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(599453583, "railgun/m1", Weapons.GlobalItems.Railgun);
        public Entity RailgunM1gold { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-1704191570, "railgun/m1gold", Weapons.GlobalItems.Railgun);
        public Entity RailgunM1steel { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-59670077, "railgun/m1steel", Weapons.GlobalItems.Railgun);
        public Entity RailgunM2 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(599479587, "railgun/m2", Weapons.GlobalItems.Railgun);
        public Entity RailgunM2tsk { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(254111980, "railgun/m2tsk", Weapons.GlobalItems.Railgun);
        public Entity RailgunMay2017 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(98615071, "railgun/may2017", Weapons.GlobalItems.Railgun);
        public Entity RailgunXt { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(593679593, "railgun/xt", Weapons.GlobalItems.Railgun);
        public Entity RailgunXtthor { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1793185296, "railgun/xt_thor", Weapons.GlobalItems.Railgun);
        public Entity RicochetFrontierzero { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1077283204, "ricochet/frontierzero", Weapons.GlobalItems.Ricochet);
        public Entity RicochetM0 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-909167727, "ricochet/m0", Weapons.GlobalItems.Ricochet, isDefault: true);
        public Entity RicochetM1 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-909167726, "ricochet/m1", Weapons.GlobalItems.Ricochet);
        public Entity RicochetM1gold { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-475547245, "ricochet/m1gold", Weapons.GlobalItems.Ricochet);
        public Entity RicochetM1steel { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-1849888712, "ricochet/m1steel", Weapons.GlobalItems.Ricochet);
        public Entity RicochetM2 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-342766924, "ricochet/m2", Weapons.GlobalItems.Ricochet);
        public Entity ShaftM0 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1460259970, "shaft/m0", Weapons.GlobalItems.Shaft, isDefault: true);
        public Entity ShaftM1 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1460259971, "shaft/m1", Weapons.GlobalItems.Shaft);
        public Entity ShaftM1gold { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(54173794, "shaft/m1gold", Weapons.GlobalItems.Shaft);
        public Entity ShaftM1steel { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-445855153, "shaft/m1steel", Weapons.GlobalItems.Shaft);
        public Entity ShaftM2 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1099854083, "shaft/m2", Weapons.GlobalItems.Shaft);
        public Entity ShaftT2 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1099854300, "shaft/t2", Weapons.GlobalItems.Shaft);
        public Entity Smoky23february2017 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-1141123658, "smoky/23february2017", Weapons.GlobalItems.Smoky);
        public Entity SmokyM0 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(2008385753, "smoky/m0", Weapons.GlobalItems.Smoky, isDefault: true);
        public Entity SmokyM1 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(2008385754, "smoky/m1", Weapons.GlobalItems.Smoky);
        public Entity SmokyM1gold { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(2004785941, "smoky/m1gold", Weapons.GlobalItems.Smoky);
        public Entity SmokyM1steel { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-617862056, "smoky/m1steel", Weapons.GlobalItems.Smoky);
        public Entity SmokyM2 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1647979866, "smoky/m2", Weapons.GlobalItems.Smoky);
        public Entity SmokyMay2017 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-1398640396, "smoky/may2017", Weapons.GlobalItems.Smoky);
        public Entity SmokyXt { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1647980273, "smoky/xt", Weapons.GlobalItems.Smoky);
        public Entity SmokyXtthor { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(295929829, "smoky/xt_thor", Weapons.GlobalItems.Smoky);
        public Entity ThunderDreadnought { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1770762539, "thunder/dreadnought", Weapons.GlobalItems.Thunder);
        public Entity ThunderM0 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1552497496, "thunder/m0", Weapons.GlobalItems.Thunder, isDefault: true);
        public Entity ThunderM1 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1552497497, "thunder/m1", Weapons.GlobalItems.Thunder);
        public Entity ThunderM1gold { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-398757768, "thunder/m1gold", Weapons.GlobalItems.Thunder);
        public Entity ThunderM1steel { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1754072121, "thunder/m1steel", Weapons.GlobalItems.Thunder);
        public Entity ThunderM2 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(2097643731, "thunder/m2", Weapons.GlobalItems.Thunder);
        public Entity ThunderMay2017 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1912357269, "thunder/may2017", Weapons.GlobalItems.Thunder);
        public Entity ThunderXt { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(2097644138, "thunder/xt", Weapons.GlobalItems.Thunder);
        public Entity ThunderXtthor { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-688039802, "thunder/xt_thor", Weapons.GlobalItems.Thunder);
        public Entity TwinsM0 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1468232592, "twins/m0", Weapons.GlobalItems.Twins, isDefault: true);
        public Entity TwinsM1 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(274558103, "twins/m1", Weapons.GlobalItems.Twins);
        public Entity TwinsM1gold { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-1595894991, "twins/m1gold", Weapons.GlobalItems.Twins);
        public Entity TwinsM1steel { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-58379936, "twins/m1steel", Weapons.GlobalItems.Twins);
        public Entity TwinsM2 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-788257966, "twins/m2", Weapons.GlobalItems.Twins);
        public Entity TwinsSteam { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-1959015927, "twins/steam", Weapons.GlobalItems.Twins);
        public Entity TwinsXt { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-788257559, "twins/xt", Weapons.GlobalItems.Twins);
        public Entity TwinsXtthor { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(855411949, "twins/xt_thor", Weapons.GlobalItems.Twins);
        public Entity VulcanCry { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-388976915, "vulcan/cry", Weapons.GlobalItems.Vulcan);
        public Entity VulcanCrydischarge { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-785177863, "vulcan/crydischarge", Weapons.GlobalItems.Vulcan);
        public Entity VulcanCryglow { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-1559746012, "vulcan/cryglow", Weapons.GlobalItems.Vulcan);
        public Entity VulcanCryrage { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-1560119161, "vulcan/cryrage", Weapons.GlobalItems.Vulcan);
        public Entity VulcanM0 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-851288667, "vulcan/m0", Weapons.GlobalItems.Vulcan, isDefault: true);
        public Entity VulcanM1 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-851288666, "vulcan/m1", Weapons.GlobalItems.Vulcan);
        public Entity VulcanM1gold { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(1066986983, "vulcan/m1gold", Weapons.GlobalItems.Vulcan);
        public Entity VulcanM1steel { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(2065196452, "vulcan/m1steel", Weapons.GlobalItems.Vulcan);
        public Entity VulcanM2 { get; private set; } = WeaponSkinMarketItemTemplate.CreateEntity(-140924792, "vulcan/m2", Weapons.GlobalItems.Vulcan);
    }
}
