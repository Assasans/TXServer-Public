using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class WeaponSkins
    {
        public static readonly Entity FlamethrowerDreadnought = new Entity(602602131, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/Dreadnought"),
            new ParentGroupComponent(Weapons.Flamethrower),
            new MarketItemGroupComponent(602602131));
        public static readonly Entity FlamethrowerM0 = new Entity(2078566312, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/m0"),
            new ParentGroupComponent(Weapons.Flamethrower),
            new MarketItemGroupComponent(2078566312));
        public static readonly Entity FlamethrowerM1 = new Entity(2078566313, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/m1"),
            new ParentGroupComponent(Weapons.Flamethrower),
            new MarketItemGroupComponent(2078566313));
        public static readonly Entity FlamethrowerM1gold = new Entity(1491390890, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/m1gold"),
            new ParentGroupComponent(Weapons.Flamethrower),
            new MarketItemGroupComponent(1491390890));
        public static readonly Entity FlamethrowerM1steel = new Entity(-1000298047, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/m1steel"),
            new ParentGroupComponent(Weapons.Flamethrower),
            new MarketItemGroupComponent(-1000298047));
        public static readonly Entity FlamethrowerM2 = new Entity(1045726795, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/m2"),
            new ParentGroupComponent(Weapons.Flamethrower),
            new MarketItemGroupComponent(1045726795));
        public static readonly Entity FlamethrowerM2flame = new Entity(-983916600, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/m2flame"),
            new ParentGroupComponent(Weapons.Flamethrower),
            new MarketItemGroupComponent(-983916600));
        public static readonly Entity FreezeM0 = new Entity(-472765007, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/freeze/m0"),
            new ParentGroupComponent(Weapons.Freeze),
            new MarketItemGroupComponent(-472765007));
        public static readonly Entity FreezeM1 = new Entity(-472765006, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/freeze/m1"),
            new ParentGroupComponent(Weapons.Freeze),
            new MarketItemGroupComponent(-472765006));
        public static readonly Entity FreezeM1gold = new Entity(-362162189, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/freeze/m1gold"),
            new ParentGroupComponent(Weapons.Freeze),
            new MarketItemGroupComponent(-362162189));
        public static readonly Entity FreezeM1steel = new Entity(1669098648, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/freeze/m1steel"),
            new ParentGroupComponent(Weapons.Freeze),
            new MarketItemGroupComponent(1669098648));
        public static readonly Entity FreezeM2 = new Entity(237598868, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/freeze/m2"),
            new ParentGroupComponent(Weapons.Freeze),
            new MarketItemGroupComponent(237598868));
        public static readonly Entity FreezeXmas = new Entity(139596834, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/freeze/xmas"),
            new ParentGroupComponent(Weapons.Freeze),
            new MarketItemGroupComponent(139596834));
        public static readonly Entity HammerCry = new Entity(283356827, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/cry"),
            new ParentGroupComponent(Weapons.Hammer),
            new MarketItemGroupComponent(283356827));
        public static readonly Entity HammerCrydischarge = new Entity(-350323544, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/crydischarge"),
            new ParentGroupComponent(Weapons.Hammer),
            new MarketItemGroupComponent(-350323544));
        public static readonly Entity HammerCryglow = new Entity(-350323543, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/cryglow"),
            new ParentGroupComponent(Weapons.Hammer),
            new MarketItemGroupComponent(-350323543));
        public static readonly Entity HammerCryrage = new Entity(-350323545, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/cryrage"),
            new ParentGroupComponent(Weapons.Hammer),
            new MarketItemGroupComponent(-350323545));
        public static readonly Entity HammerM0 = new Entity(-635589854, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/m0"),
            new ParentGroupComponent(Weapons.Hammer),
            new MarketItemGroupComponent(-635589854));
        public static readonly Entity HammerM1 = new Entity(-635589853, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/m1"),
            new ParentGroupComponent(Weapons.Hammer),
            new MarketItemGroupComponent(-635589853));
        public static readonly Entity HammerM1gold = new Entity(-1427688220, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/m1gold"),
            new ParentGroupComponent(Weapons.Hammer),
            new MarketItemGroupComponent(-1427688220));
        public static readonly Entity HammerM1steel = new Entity(2039676487, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/m1steel"),
            new ParentGroupComponent(Weapons.Hammer),
            new MarketItemGroupComponent(2039676487));
        public static readonly Entity HammerM2 = new Entity(1076729477, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/m2"),
            new ParentGroupComponent(Weapons.Hammer),
            new MarketItemGroupComponent(1076729477));
        public static readonly Entity Isis8march2017 = new Entity(-305773183, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/isis/8march2017"),
            new ParentGroupComponent(Weapons.Isis),
            new MarketItemGroupComponent(-305773183));
        public static readonly Entity IsisM0 = new Entity(-746649388, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/isis/m0"),
            new ParentGroupComponent(Weapons.Isis),
            new MarketItemGroupComponent(-746649388));
        public static readonly Entity IsisM1 = new Entity(-746649387, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/isis/m1"),
            new ParentGroupComponent(Weapons.Isis),
            new MarketItemGroupComponent(-746649387));
        public static readonly Entity IsisM1gold = new Entity(-1704481322, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/isis/m1gold"),
            new ParentGroupComponent(Weapons.Isis),
            new MarketItemGroupComponent(-1704481322));
        public static readonly Entity IsisM1steel = new Entity(-691629355, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/isis/m1steel"),
            new ParentGroupComponent(Weapons.Isis),
            new MarketItemGroupComponent(-691629355));
        public static readonly Entity IsisM2 = new Entity(765745271, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/isis/m2"),
            new ParentGroupComponent(Weapons.Isis),
            new MarketItemGroupComponent(765745271));
        public static readonly Entity RailgunM0 = new Entity(599453582, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/m0"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(599453582));
        public static readonly Entity RailgunM1 = new Entity(599453583, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/m1"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(599453583));
        public static readonly Entity RailgunM1gold = new Entity(-1704191570, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/m1gold"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(-1704191570));
        public static readonly Entity RailgunM1steel = new Entity(-59670077, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/m1steel"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(-59670077));
        public static readonly Entity RailgunM2 = new Entity(599479587, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/m2"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(599479587));
        public static readonly Entity RailgunM2tsk = new Entity(254111980, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/m2tsk"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(254111980));
        public static readonly Entity RailgunMay2017 = new Entity(98615071, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/may2017"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(98615071));
        public static readonly Entity RailgunXt = new Entity(593679593, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/xt"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(593679593));
        public static readonly Entity RailgunXt_thor = new Entity(1793185296, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/xt_thor"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(1793185296));
        public static readonly Entity RicochetFrontierzero = new Entity(1077283204, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/ricochet/frontierzero"),
            new ParentGroupComponent(Weapons.Ricochet),
            new MarketItemGroupComponent(1077283204));
        public static readonly Entity RicochetM0 = new Entity(-909167727, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/ricochet/m0"),
            new ParentGroupComponent(Weapons.Ricochet),
            new MarketItemGroupComponent(-909167727));
        public static readonly Entity RicochetM1 = new Entity(-909167726, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/ricochet/m1"),
            new ParentGroupComponent(Weapons.Ricochet),
            new MarketItemGroupComponent(-909167726));
        public static readonly Entity RicochetM1gold = new Entity(-475547245, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/ricochet/m1gold"),
            new ParentGroupComponent(Weapons.Ricochet),
            new MarketItemGroupComponent(-475547245));
        public static readonly Entity RicochetM1steel = new Entity(-1849888712, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/ricochet/m1steel"),
            new ParentGroupComponent(Weapons.Ricochet),
            new MarketItemGroupComponent(-1849888712));
        public static readonly Entity RicochetM2 = new Entity(-342766924, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/ricochet/m2"),
            new ParentGroupComponent(Weapons.Ricochet),
            new MarketItemGroupComponent(-342766924));
        public static readonly Entity ShaftM0 = new Entity(1460259970, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/shaft/m0"),
            new ParentGroupComponent(Weapons.Shaft),
            new MarketItemGroupComponent(1460259970));
        public static readonly Entity ShaftM1 = new Entity(1460259971, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/shaft/m1"),
            new ParentGroupComponent(Weapons.Shaft),
            new MarketItemGroupComponent(1460259971));
        public static readonly Entity ShaftM1gold = new Entity(54173794, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/shaft/m1gold"),
            new ParentGroupComponent(Weapons.Shaft),
            new MarketItemGroupComponent(54173794));
        public static readonly Entity ShaftM1steel = new Entity(-445855153, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/shaft/m1steel"),
            new ParentGroupComponent(Weapons.Shaft),
            new MarketItemGroupComponent(-445855153));
        public static readonly Entity ShaftM2 = new Entity(1099854083, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/shaft/m2"),
            new ParentGroupComponent(Weapons.Shaft),
            new MarketItemGroupComponent(1099854083));
        public static readonly Entity ShaftT2 = new Entity(1099854300, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/shaft/t2"),
            new ParentGroupComponent(Weapons.Shaft),
            new MarketItemGroupComponent(1099854300));
        public static readonly Entity Smoky23february2017 = new Entity(-1141123658, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/23february2017"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(-1141123658));
        public static readonly Entity SmokyM0 = new Entity(2008385753, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/m0"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(2008385753));
        public static readonly Entity SmokyM1 = new Entity(2008385754, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/m1"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(2008385754));
        public static readonly Entity SmokyM1gold = new Entity(2004785941, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/m1gold"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(2004785941));
        public static readonly Entity SmokyM1steel = new Entity(-617862056, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/m1steel"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(-617862056));
        public static readonly Entity SmokyM2 = new Entity(1647979866, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/m2"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(1647979866));
        public static readonly Entity SmokyMay2017 = new Entity(-1398640396, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/may2017"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(-1398640396));
        public static readonly Entity SmokyXt = new Entity(1647980273, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/xt"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(1647980273));
        public static readonly Entity SmokyXt_thor = new Entity(295929829, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/xt_thor"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(295929829));
        public static readonly Entity ThunderDreadnought = new Entity(1770762539, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/dreadnought"),
            new ParentGroupComponent(Weapons.Thunder),
            new MarketItemGroupComponent(1770762539));
        public static readonly Entity ThunderM0 = new Entity(1552497496, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/m0"),
            new ParentGroupComponent(Weapons.Thunder),
            new MarketItemGroupComponent(1552497496));
        public static readonly Entity ThunderM1 = new Entity(1552497497, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/m1"),
            new ParentGroupComponent(Weapons.Thunder),
            new MarketItemGroupComponent(1552497497));
        public static readonly Entity ThunderM1gold = new Entity(-398757768, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/m1gold"),
            new ParentGroupComponent(Weapons.Thunder),
            new MarketItemGroupComponent(-398757768));
        public static readonly Entity ThunderM1steel = new Entity(1754072121, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/m1steel"),
            new ParentGroupComponent(Weapons.Thunder),
            new MarketItemGroupComponent(1754072121));
        public static readonly Entity ThunderM2 = new Entity(2097643731, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/m2"),
            new ParentGroupComponent(Weapons.Thunder),
            new MarketItemGroupComponent(2097643731));
        public static readonly Entity ThunderMay2017 = new Entity(1912357269, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/may2017"),
            new ParentGroupComponent(Weapons.Thunder),
            new MarketItemGroupComponent(1912357269));
        public static readonly Entity ThunderXt = new Entity(2097644138, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/xt"),
            new ParentGroupComponent(Weapons.Thunder),
            new MarketItemGroupComponent(2097644138));
        public static readonly Entity ThunderXt_thor = new Entity(-688039802, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/xt_thor"),
            new ParentGroupComponent(Weapons.Thunder),
            new MarketItemGroupComponent(-688039802));
        public static readonly Entity TwinsM0 = new Entity(1468232592, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/m0"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(1468232592));
        public static readonly Entity TwinsM1 = new Entity(274558103, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/m1"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(274558103));
        public static readonly Entity TwinsM1gold = new Entity(-1595894991, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/m1gold"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(-1595894991));
        public static readonly Entity TwinsM1steel = new Entity(-58379936, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/m1steel"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(-58379936));
        public static readonly Entity TwinsM2 = new Entity(-788257966, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/m2"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(-788257966));
        public static readonly Entity TwinsSteam = new Entity(-1959015927, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/steam"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(-1959015927));
        public static readonly Entity TwinsXt = new Entity(-788257559, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/xt"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(-788257559));
        public static readonly Entity TwinsXt_thor = new Entity(855411949, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/xt_thor"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(855411949));
        public static readonly Entity VulcanCry = new Entity(-388976915, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/cry"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(-388976915));
        public static readonly Entity VulcanCrydischarge = new Entity(-785177863, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/crydischarge"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(-785177863));
        public static readonly Entity VulcanCryglow = new Entity(-1559746012, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/cryglow"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(-1559746012));
        public static readonly Entity VulcanCryrage = new Entity(-1560119161, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/cryrage"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(-1560119161));
        public static readonly Entity VulcanM0 = new Entity(-851288667, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/m0"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(-851288667));
        public static readonly Entity VulcanM1 = new Entity(-851288666, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/m1"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(-851288666));
        public static readonly Entity VulcanM1gold = new Entity(1066986983, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/m1gold"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(1066986983));
        public static readonly Entity VulcanM1steel = new Entity(2065196452, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/m1steel"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(2065196452));
        public static readonly Entity VulcanM2 = new Entity(-140924792, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/m2"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(-140924792));
    }
}
