using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Shells
    {
        public static readonly Entity FlamethrowerAcid = new Entity(692677861, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/flamethrower/acid"),
            new ParentGroupComponent(Weapons.Flamethrower),
            new MarketItemGroupComponent(692677861));
        public static readonly Entity FlamethrowerOrange = new Entity(357929046, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/flamethrower/orange"),
            new ParentGroupComponent(Weapons.Flamethrower),
            new MarketItemGroupComponent(357929046));
        public static readonly Entity FlamethrowerRed = new Entity(-947470487, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/flamethrower/red"),
            new ParentGroupComponent(Weapons.Flamethrower),
            new MarketItemGroupComponent(-947470487));
        public static readonly Entity FreezeIndigo = new Entity(224610499, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/freeze/indigo"),
            new ParentGroupComponent(Weapons.Freeze),
            new MarketItemGroupComponent(224610499));
        public static readonly Entity FreezeSkyBlue = new Entity(-1408603862, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/freeze/skyblue"),
            new ParentGroupComponent(Weapons.Freeze),
            new MarketItemGroupComponent(-1408603862));
        public static readonly Entity FreezeWhite = new Entity(-395640808, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/freeze/white"),
            new ParentGroupComponent(Weapons.Freeze),
            new MarketItemGroupComponent(-395640808));
        public static readonly Entity HammerPapercracker = new Entity(142652989, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/hammer/papercracker"),
            new ParentGroupComponent(Weapons.Hammer),
            new MarketItemGroupComponent(142652989));
        public static readonly Entity HammerStandard = new Entity(530945311, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/hammer/standard"),
            new ParentGroupComponent(Weapons.Hammer),
            new MarketItemGroupComponent(530945311));
        public static readonly Entity HammerThor = new Entity(1317881529, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/hammer/thor"),
            new ParentGroupComponent(Weapons.Hammer),
            new MarketItemGroupComponent(1317881529));
        public static readonly Entity IsisEmerald = new Entity(2065928272, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/isis/emerald"),
            new ParentGroupComponent(Weapons.Isis),
            new MarketItemGroupComponent(2065928272));
        public static readonly Entity IsisShine = new Entity(-132552041, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/isis/shine"),
            new ParentGroupComponent(Weapons.Isis),
            new MarketItemGroupComponent(132552041));
        public static readonly Entity IsisStandard = new Entity(48235025, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/isis/standard"),
            new ParentGroupComponent(Weapons.Isis),
            new MarketItemGroupComponent(48235025));
        public static readonly Entity RailgunGlitch = new Entity(-1975536348, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/glitch"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(-1975536348));
        public static readonly Entity RailgunGloom = new Entity(-375403918, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/gloom"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(-375403918));
        public static readonly Entity RailgunGreen = new Entity(-660457061, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/green"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(-660457061));
        public static readonly Entity RailgunOrange = new Entity(1229590166, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/orange"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(1229590166));
        public static readonly Entity RailgunPaleBlue = new Entity(366763244, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/paleblue"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(366763244));
        public static readonly Entity RailgunPurple = new Entity(1261498404, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/purple"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(1261498404));
        public static readonly Entity RailgunSmoke = new Entity(-1195981301, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/smoke"),
            new ParentGroupComponent(Weapons.Railgun),
            new MarketItemGroupComponent(-1195981301));
        public static readonly Entity RicochetAcid = new Entity(-492753567, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/ricochet/acid"),
            new ParentGroupComponent(Weapons.Ricochet),
            new MarketItemGroupComponent(-492753567));
        public static readonly Entity RicochetAurulent = new Entity(139800007, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/ricochet/aurulent"),
            new ParentGroupComponent(Weapons.Ricochet),
            new MarketItemGroupComponent(139800007));
        public static readonly Entity RicochetCannonBall = new Entity(-1899861235, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/ricochet/cannonball"),
            new ParentGroupComponent(Weapons.Ricochet),
            new MarketItemGroupComponent(-1899861235));
        public static readonly Entity RicochetCoral = new Entity(577198848, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/ricochet/coral"),
            new ParentGroupComponent(Weapons.Ricochet),
            new MarketItemGroupComponent(577198848));
        public static readonly Entity Ricochet = new Entity(-585038633, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/ricochet/moon"),
            new ParentGroupComponent(Weapons.Ricochet),
            new MarketItemGroupComponent(-585038633));
        public static readonly Entity RicocherRicSnowball = new Entity(-1073342998, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/ricochet/ricsnowball"),
            new ParentGroupComponent(Weapons.Ricochet),
            new MarketItemGroupComponent(-1073342998));
        public static readonly Entity ShaftBlue = new Entity(-1488314890, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/shaft/blue"),
            new ParentGroupComponent(Weapons.Shaft),
            new MarketItemGroupComponent(-1488314890));
        public static readonly Entity ShaftLighting = new Entity(-250346840, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/shaft/lighting"),
            new ParentGroupComponent(Weapons.Shaft),
            new MarketItemGroupComponent(-250346840));
        public static readonly Entity ShaftRed = new Entity(-47995019, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/shaft/red"),
            new ParentGroupComponent(Weapons.Shaft),
            new MarketItemGroupComponent(-47995019));
        public static readonly Entity ShaftStandard = new Entity(70311513, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/shaft/standard"),
            new ParentGroupComponent(Weapons.Shaft),
            new MarketItemGroupComponent(70311513));
        public static readonly Entity SmokyFumes = new Entity(860375257, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/smoky/fumes"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(860375257));
        public static readonly Entity SmokyLightning = new Entity(-1260549513, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/smoky/lightning"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(-1260549513));
        public static readonly Entity SmokyStandard = new Entity(-966935184, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/smoky/standard"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(-966935184));
        public static readonly Entity SmokyThermite = new Entity(-1165747871, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/smoky/thermite"),
            new ParentGroupComponent(Weapons.Smoky),
            new MarketItemGroupComponent(-1165747871));
        public static readonly Entity ThunderDragon = new Entity(-1852311024, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/thunder/dragon"),
            new ParentGroupComponent(Weapons.Thunder),
            new MarketItemGroupComponent(-1852311024));
        public static readonly Entity ThunderLightning = new Entity(794635633, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/thunder/lightning"),
            new ParentGroupComponent(Weapons.Thunder),
            new MarketItemGroupComponent(794635633));
        public static readonly Entity ThunderStandard = new Entity(1067800943, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/thunder/standard"),
            new ParentGroupComponent(Weapons.Thunder),
            new MarketItemGroupComponent(1067800943));
        public static readonly Entity TwinsBlue = new Entity(807172229, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/twins/blue"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(807172229));
        public static readonly Entity TwinsOrange = new Entity(-1319432295, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/twins/orange"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(-1319432295));
        public static readonly Entity TwinsTurquoise = new Entity(-1173768746, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/twins/turquoise"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(-1173768746));
        public static readonly Entity TwinsTwinsSnowball = new Entity(-1092514341, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/twins/twinssnowball"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(-1092514341));
        public static readonly Entity TwinsViolet = new Entity(-1126924822, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/twins/violet"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(-1126924822));
        public static readonly Entity TwinsWhite = new Entity(-728200866, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/twins/white"),
            new ParentGroupComponent(Weapons.Twins),
            new MarketItemGroupComponent(-728200866));
        public static readonly Entity VulcanAcid = new Entity(189181410, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/vulcan/acid"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(189181410));
        public static readonly Entity VulcanBlue = new Entity(189220223, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/vulcan/blue"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(189220223));
        public static readonly Entity VulcanRed = new Entity(-963712308, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/vulcan/red"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(-963712308));
        public static readonly Entity VulcanStandard = new Entity(1322064226, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/vulcan/standard"),
            new ParentGroupComponent(Weapons.Vulcan),
            new MarketItemGroupComponent(1322064226));
    }
}
