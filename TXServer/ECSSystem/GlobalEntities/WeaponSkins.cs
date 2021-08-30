using System.Linq;
using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class WeaponSkins
    {
        public static Items GlobalItems { get; } = new Items();

        public static Items GetUserItems(Player player)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                long id = item.EntityId;
                item.EntityId = Entity.GenerateId();

                item.TemplateAccessor.Template = new WeaponSkinUserItemTemplate();

                if (player.Data.Weapons.SelectMany(weapon => weapon.Skins).ToIds().Contains(id))
                    item.Components.Add(new UserGroupComponent(player.User));
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity FlamethrowerDreadnought { get; } = new Entity(602602131, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/Dreadnought"),
                new ParentGroupComponent(Weapons.GlobalItems.Flamethrower),
                new MarketItemGroupComponent(602602131));
            public Entity FlamethrowerM0 { get; } = new Entity(2078566312, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/m0"),
                new ParentGroupComponent(Weapons.GlobalItems.Flamethrower),
                new MarketItemGroupComponent(2078566312),
                new DefaultSkinItemComponent());
            public Entity FlamethrowerM1 { get; } = new Entity(2078566313, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/m1"),
                new ParentGroupComponent(Weapons.GlobalItems.Flamethrower),
                new MarketItemGroupComponent(2078566313));
            public Entity FlamethrowerM1gold { get; } = new Entity(1491390890, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/m1gold"),
                new ParentGroupComponent(Weapons.GlobalItems.Flamethrower),
                new MarketItemGroupComponent(1491390890));
            public Entity FlamethrowerM1steel { get; } = new Entity(-1000298047, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/m1steel"),
                new ParentGroupComponent(Weapons.GlobalItems.Flamethrower),
                new MarketItemGroupComponent(-1000298047));
            public Entity FlamethrowerM2 { get; } = new Entity(1045726795, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/m2"),
                new ParentGroupComponent(Weapons.GlobalItems.Flamethrower),
                new MarketItemGroupComponent(1045726795));
            public Entity FlamethrowerM2flame { get; } = new Entity(-983916600, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/flamethrower/m2flame"),
                new ParentGroupComponent(Weapons.GlobalItems.Flamethrower),
                new MarketItemGroupComponent(-983916600));
            public Entity FreezeM0 { get; } = new Entity(-472765007, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/freeze/m0"),
                new ParentGroupComponent(Weapons.GlobalItems.Freeze),
                new MarketItemGroupComponent(-472765007),
                new DefaultSkinItemComponent());
            public Entity FreezeM1 { get; } = new Entity(-472765006, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/freeze/m1"),
                new ParentGroupComponent(Weapons.GlobalItems.Freeze),
                new MarketItemGroupComponent(-472765006));
            public Entity FreezeM1gold { get; } = new Entity(-362162189, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/freeze/m1gold"),
                new ParentGroupComponent(Weapons.GlobalItems.Freeze),
                new MarketItemGroupComponent(-362162189));
            public Entity FreezeM1steel { get; } = new Entity(1669098648, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/freeze/m1steel"),
                new ParentGroupComponent(Weapons.GlobalItems.Freeze),
                new MarketItemGroupComponent(1669098648));
            public Entity FreezeM2 { get; } = new Entity(237598868, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/freeze/m2"),
                new ParentGroupComponent(Weapons.GlobalItems.Freeze),
                new MarketItemGroupComponent(237598868));
            public Entity FreezeXmas { get; } = new Entity(139596834, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/freeze/xmas"),
                new ParentGroupComponent(Weapons.GlobalItems.Freeze),
                new MarketItemGroupComponent(139596834));
            public Entity HammerCry { get; } = new Entity(283356827, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/cry"),
                new ParentGroupComponent(Weapons.GlobalItems.Hammer),
                new MarketItemGroupComponent(283356827));
            public Entity HammerCrydischarge { get; } = new Entity(-350323544, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/crydischarge"),
                new ParentGroupComponent(Weapons.GlobalItems.Hammer),
                new MarketItemGroupComponent(-350323544));
            public Entity HammerCryglow { get; } = new Entity(-350323543, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/cryglow"),
                new ParentGroupComponent(Weapons.GlobalItems.Hammer),
                new MarketItemGroupComponent(-350323543));
            public Entity HammerCryrage { get; } = new Entity(-350323545, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/cryrage"),
                new ParentGroupComponent(Weapons.GlobalItems.Hammer),
                new MarketItemGroupComponent(-350323545));
            public Entity HammerM0 { get; } = new Entity(-635589854, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/m0"),
                new ParentGroupComponent(Weapons.GlobalItems.Hammer),
                new MarketItemGroupComponent(-635589854),
                new DefaultSkinItemComponent());
            public Entity HammerM1 { get; } = new Entity(-635589853, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/m1"),
                new ParentGroupComponent(Weapons.GlobalItems.Hammer),
                new MarketItemGroupComponent(-635589853));
            public Entity HammerM1gold { get; } = new Entity(-1427688220, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/m1gold"),
                new ParentGroupComponent(Weapons.GlobalItems.Hammer),
                new MarketItemGroupComponent(-1427688220));
            public Entity HammerM1steel { get; } = new Entity(2039676487, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/m1steel"),
                new ParentGroupComponent(Weapons.GlobalItems.Hammer),
                new MarketItemGroupComponent(2039676487));
            public Entity HammerM2 { get; } = new Entity(1076729477, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/hammer/m2"),
                new ParentGroupComponent(Weapons.GlobalItems.Hammer),
                new MarketItemGroupComponent(1076729477));
            public Entity Isis8march2017 { get; } = new Entity(-305773183, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/isis/8march2017"),
                new ParentGroupComponent(Weapons.GlobalItems.Isis),
                new MarketItemGroupComponent(-305773183));
            public Entity IsisM0 { get; } = new Entity(-746649388, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/isis/m0"),
                new ParentGroupComponent(Weapons.GlobalItems.Isis),
                new MarketItemGroupComponent(-746649388),
                new DefaultSkinItemComponent());
            public Entity IsisM1 { get; } = new Entity(-746649387, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/isis/m1"),
                new ParentGroupComponent(Weapons.GlobalItems.Isis),
                new MarketItemGroupComponent(-746649387));
            public Entity IsisM1gold { get; } = new Entity(-1704481322, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/isis/m1gold"),
                new ParentGroupComponent(Weapons.GlobalItems.Isis),
                new MarketItemGroupComponent(-1704481322));
            public Entity IsisM1steel { get; } = new Entity(-691629355, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/isis/m1steel"),
                new ParentGroupComponent(Weapons.GlobalItems.Isis),
                new MarketItemGroupComponent(-691629355));
            public Entity IsisM2 { get; } = new Entity(765745271, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/isis/m2"),
                new ParentGroupComponent(Weapons.GlobalItems.Isis),
                new MarketItemGroupComponent(765745271));
            public Entity RailgunM0 { get; } = new Entity(599453582, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/m0"),
                new ParentGroupComponent(Weapons.GlobalItems.Railgun),
                new MarketItemGroupComponent(599453582),
                new DefaultSkinItemComponent());
            public Entity RailgunM1 { get; } = new Entity(599453583, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/m1"),
                new ParentGroupComponent(Weapons.GlobalItems.Railgun),
                new MarketItemGroupComponent(599453583));
            public Entity RailgunM1gold { get; } = new Entity(-1704191570, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/m1gold"),
                new ParentGroupComponent(Weapons.GlobalItems.Railgun),
                new MarketItemGroupComponent(-1704191570));
            public Entity RailgunM1steel { get; } = new Entity(-59670077, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/m1steel"),
                new ParentGroupComponent(Weapons.GlobalItems.Railgun),
                new MarketItemGroupComponent(-59670077));
            public Entity RailgunM2 { get; } = new Entity(599479587, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/m2"),
                new ParentGroupComponent(Weapons.GlobalItems.Railgun),
                new MarketItemGroupComponent(599479587));
            public Entity RailgunM2tsk { get; } = new Entity(254111980, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/m2tsk"),
                new ParentGroupComponent(Weapons.GlobalItems.Railgun),
                new MarketItemGroupComponent(254111980));
            public Entity RailgunMay2017 { get; } = new Entity(98615071, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/may2017"),
                new ParentGroupComponent(Weapons.GlobalItems.Railgun),
                new MarketItemGroupComponent(98615071));
            public Entity RailgunXt { get; } = new Entity(593679593, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/xt"),
                new ParentGroupComponent(Weapons.GlobalItems.Railgun),
                new MarketItemGroupComponent(593679593));
            public Entity RailgunXt_thor { get; } = new Entity(1793185296, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/railgun/xt_thor"),
                new ParentGroupComponent(Weapons.GlobalItems.Railgun),
                new MarketItemGroupComponent(1793185296));
            public Entity RicochetFrontierzero { get; } = new Entity(1077283204, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/ricochet/frontierzero"),
                new ParentGroupComponent(Weapons.GlobalItems.Ricochet),
                new MarketItemGroupComponent(1077283204));
            public Entity RicochetM0 { get; } = new Entity(-909167727, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/ricochet/m0"),
                new ParentGroupComponent(Weapons.GlobalItems.Ricochet),
                new MarketItemGroupComponent(-909167727),
                new DefaultSkinItemComponent());
            public Entity RicochetM1 { get; } = new Entity(-909167726, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/ricochet/m1"),
                new ParentGroupComponent(Weapons.GlobalItems.Ricochet),
                new MarketItemGroupComponent(-909167726));
            public Entity RicochetM1gold { get; } = new Entity(-475547245, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/ricochet/m1gold"),
                new ParentGroupComponent(Weapons.GlobalItems.Ricochet),
                new MarketItemGroupComponent(-475547245));
            public Entity RicochetM1steel { get; } = new Entity(-1849888712, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/ricochet/m1steel"),
                new ParentGroupComponent(Weapons.GlobalItems.Ricochet),
                new MarketItemGroupComponent(-1849888712));
            public Entity RicochetM2 { get; } = new Entity(-342766924, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/ricochet/m2"),
                new ParentGroupComponent(Weapons.GlobalItems.Ricochet),
                new MarketItemGroupComponent(-342766924));
            public Entity ShaftM0 { get; } = new Entity(1460259970, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/shaft/m0"),
                new ParentGroupComponent(Weapons.GlobalItems.Shaft),
                new MarketItemGroupComponent(1460259970),
                new DefaultSkinItemComponent());
            public Entity ShaftM1 { get; } = new Entity(1460259971, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/shaft/m1"),
                new ParentGroupComponent(Weapons.GlobalItems.Shaft),
                new MarketItemGroupComponent(1460259971));
            public Entity ShaftM1gold { get; } = new Entity(54173794, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/shaft/m1gold"),
                new ParentGroupComponent(Weapons.GlobalItems.Shaft),
                new MarketItemGroupComponent(54173794));
            public Entity ShaftM1steel { get; } = new Entity(-445855153, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/shaft/m1steel"),
                new ParentGroupComponent(Weapons.GlobalItems.Shaft),
                new MarketItemGroupComponent(-445855153));
            public Entity ShaftM2 { get; } = new Entity(1099854083, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/shaft/m2"),
                new ParentGroupComponent(Weapons.GlobalItems.Shaft),
                new MarketItemGroupComponent(1099854083));
            public Entity ShaftT2 { get; } = new Entity(1099854300, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/shaft/t2"),
                new ParentGroupComponent(Weapons.GlobalItems.Shaft),
                new MarketItemGroupComponent(1099854300));
            public Entity Smoky23february2017 { get; } = new Entity(-1141123658, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/23february2017"),
                new ParentGroupComponent(Weapons.GlobalItems.Smoky),
                new MarketItemGroupComponent(-1141123658));
            public Entity SmokyM0 { get; } = new Entity(2008385753, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/m0"),
                new ParentGroupComponent(Weapons.GlobalItems.Smoky),
                new MarketItemGroupComponent(2008385753),
                new DefaultSkinItemComponent());
            public Entity SmokyM1 { get; } = new Entity(2008385754, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/m1"),
                new ParentGroupComponent(Weapons.GlobalItems.Smoky),
                new MarketItemGroupComponent(2008385754));
            public Entity SmokyM1gold { get; } = new Entity(2004785941, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/m1gold"),
                new ParentGroupComponent(Weapons.GlobalItems.Smoky),
                new MarketItemGroupComponent(2004785941));
            public Entity SmokyM1steel { get; } = new Entity(-617862056, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/m1steel"),
                new ParentGroupComponent(Weapons.GlobalItems.Smoky),
                new MarketItemGroupComponent(-617862056));
            public Entity SmokyM2 { get; } = new Entity(1647979866, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/m2"),
                new ParentGroupComponent(Weapons.GlobalItems.Smoky),
                new MarketItemGroupComponent(1647979866));
            public Entity SmokyMay2017 { get; } = new Entity(-1398640396, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/may2017"),
                new ParentGroupComponent(Weapons.GlobalItems.Smoky),
                new MarketItemGroupComponent(-1398640396));
            public Entity SmokyXt { get; } = new Entity(1647980273, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/xt"),
                new ParentGroupComponent(Weapons.GlobalItems.Smoky),
                new MarketItemGroupComponent(1647980273));
            public Entity SmokyXt_thor { get; } = new Entity(295929829, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/smoky/xt_thor"),
                new ParentGroupComponent(Weapons.GlobalItems.Smoky),
                new MarketItemGroupComponent(295929829));
            public Entity ThunderDreadnought { get; } = new Entity(1770762539, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/dreadnought"),
                new ParentGroupComponent(Weapons.GlobalItems.Thunder),
                new MarketItemGroupComponent(1770762539));
            public Entity ThunderM0 { get; } = new Entity(1552497496, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/m0"),
                new ParentGroupComponent(Weapons.GlobalItems.Thunder),
                new MarketItemGroupComponent(1552497496),
                new DefaultSkinItemComponent());
            public Entity ThunderM1 { get; } = new Entity(1552497497, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/m1"),
                new ParentGroupComponent(Weapons.GlobalItems.Thunder),
                new MarketItemGroupComponent(1552497497));
            public Entity ThunderM1gold { get; } = new Entity(-398757768, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/m1gold"),
                new ParentGroupComponent(Weapons.GlobalItems.Thunder),
                new MarketItemGroupComponent(-398757768));
            public Entity ThunderM1steel { get; } = new Entity(1754072121, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/m1steel"),
                new ParentGroupComponent(Weapons.GlobalItems.Thunder),
                new MarketItemGroupComponent(1754072121));
            public Entity ThunderM2 { get; } = new Entity(2097643731, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/m2"),
                new ParentGroupComponent(Weapons.GlobalItems.Thunder),
                new MarketItemGroupComponent(2097643731));
            public Entity ThunderMay2017 { get; } = new Entity(1912357269, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/may2017"),
                new ParentGroupComponent(Weapons.GlobalItems.Thunder),
                new MarketItemGroupComponent(1912357269));
            public Entity ThunderXt { get; } = new Entity(2097644138, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/xt"),
                new ParentGroupComponent(Weapons.GlobalItems.Thunder),
                new MarketItemGroupComponent(2097644138));
            public Entity ThunderXt_thor { get; } = new Entity(-688039802, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/thunder/xt_thor"),
                new ParentGroupComponent(Weapons.GlobalItems.Thunder),
                new MarketItemGroupComponent(-688039802));
            public Entity TwinsM0 { get; } = new Entity(1468232592, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/m0"),
                new ParentGroupComponent(Weapons.GlobalItems.Twins),
                new MarketItemGroupComponent(1468232592),
                new DefaultSkinItemComponent());
            public Entity TwinsM1 { get; } = new Entity(274558103, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/m1"),
                new ParentGroupComponent(Weapons.GlobalItems.Twins),
                new MarketItemGroupComponent(274558103));
            public Entity TwinsM1gold { get; } = new Entity(-1595894991, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/m1gold"),
                new ParentGroupComponent(Weapons.GlobalItems.Twins),
                new MarketItemGroupComponent(-1595894991));
            public Entity TwinsM1steel { get; } = new Entity(-58379936, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/m1steel"),
                new ParentGroupComponent(Weapons.GlobalItems.Twins),
                new MarketItemGroupComponent(-58379936));
            public Entity TwinsM2 { get; } = new Entity(-788257966, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/m2"),
                new ParentGroupComponent(Weapons.GlobalItems.Twins),
                new MarketItemGroupComponent(-788257966));
            public Entity TwinsSteam { get; } = new Entity(-1959015927, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/steam"),
                new ParentGroupComponent(Weapons.GlobalItems.Twins),
                new MarketItemGroupComponent(-1959015927));
            public Entity TwinsXt { get; } = new Entity(-788257559, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/xt"),
                new ParentGroupComponent(Weapons.GlobalItems.Twins),
                new MarketItemGroupComponent(-788257559));
            public Entity TwinsXt_thor { get; } = new Entity(855411949, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/twins/xt_thor"),
                new ParentGroupComponent(Weapons.GlobalItems.Twins),
                new MarketItemGroupComponent(855411949));
            public Entity VulcanCry { get; } = new Entity(-388976915, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/cry"),
                new ParentGroupComponent(Weapons.GlobalItems.Vulcan),
                new MarketItemGroupComponent(-388976915));
            public Entity VulcanCrydischarge { get; } = new Entity(-785177863, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/crydischarge"),
                new ParentGroupComponent(Weapons.GlobalItems.Vulcan),
                new MarketItemGroupComponent(-785177863));
            public Entity VulcanCryglow { get; } = new Entity(-1559746012, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/cryglow"),
                new ParentGroupComponent(Weapons.GlobalItems.Vulcan),
                new MarketItemGroupComponent(-1559746012));
            public Entity VulcanCryrage { get; } = new Entity(-1560119161, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/cryrage"),
                new ParentGroupComponent(Weapons.GlobalItems.Vulcan),
                new MarketItemGroupComponent(-1560119161));
            public Entity VulcanM0 { get; } = new Entity(-851288667, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/m0"),
                new ParentGroupComponent(Weapons.GlobalItems.Vulcan),
                new MarketItemGroupComponent(-851288667),
                new DefaultSkinItemComponent());
            public Entity VulcanM1 { get; } = new Entity(-851288666, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/m1"),
                new ParentGroupComponent(Weapons.GlobalItems.Vulcan),
                new MarketItemGroupComponent(-851288666));
            public Entity VulcanM1gold { get; } = new Entity(1066986983, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/m1gold"),
                new ParentGroupComponent(Weapons.GlobalItems.Vulcan),
                new MarketItemGroupComponent(1066986983));
            public Entity VulcanM1steel { get; } = new Entity(2065196452, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/m1steel"),
                new ParentGroupComponent(Weapons.GlobalItems.Vulcan),
                new MarketItemGroupComponent(2065196452));
            public Entity VulcanM2 { get; } = new Entity(-140924792, new TemplateAccessor(new WeaponSkinMarketItemTemplate(), "garage/skin/weapon/vulcan/m2"),
                new ParentGroupComponent(Weapons.GlobalItems.Vulcan),
                new MarketItemGroupComponent(-140924792));
        }
    }
}
