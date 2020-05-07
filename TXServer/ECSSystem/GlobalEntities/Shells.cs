using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Shells
    {
        public static Items GlobalItems { get; } = new Items();

        public static Items GetUserItems(Entity user)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.EntityId = Player.GenerateId();

                item.TemplateAccessor.Template = new ShellUserItemTemplate();

                item.Components.Add(new UserGroupComponent(user.EntityId));

                if (item.TemplateAccessor.ConfigPath.Contains("standard"))
                {
                    item.Components.Add(new MountedItemComponent());
                }
            }

            foreach (Entity item in new [] { items.FlamethrowerOrange,
                                             items.FreezeSkyblue,
                                             items.RailgunPaleblue,
                                             items.RicochetAurulent,
                                             items.TwinsBlue})
            {
                item.Components.Add(new MountedItemComponent());
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity FlamethrowerAcid { get; } = new Entity(692677861, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/flamethrower/acid"),
                new ParentGroupComponent(692677861),
                new MarketItemGroupComponent(692677861));
            public Entity FlamethrowerOrange { get; } = new Entity(357929046, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/flamethrower/orange"),
                new ParentGroupComponent(357929046),
                new MarketItemGroupComponent(357929046));
            public Entity FlamethrowerRed { get; } = new Entity(-947470487, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/flamethrower/red"),
                new ParentGroupComponent(-947470487),
                new MarketItemGroupComponent(-947470487));
            public Entity FreezeIndigo { get; } = new Entity(224610499, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/freeze/indigo"),
                new ParentGroupComponent(224610499),
                new MarketItemGroupComponent(224610499));
            public Entity FreezeSkyblue { get; } = new Entity(-1408603862, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/freeze/skyblue"),
                new ParentGroupComponent(-1408603862),
                new MarketItemGroupComponent(-1408603862));
            public Entity FreezeWhite { get; } = new Entity(-395640808, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/freeze/white"),
                new ParentGroupComponent(-395640808),
                new MarketItemGroupComponent(-395640808));
            public Entity HammerPapercracker { get; } = new Entity(142652989, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/hammer/papercracker"),
                new ParentGroupComponent(142652989),
                new MarketItemGroupComponent(142652989));
            public Entity HammerStandard { get; } = new Entity(530945311, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/hammer/standard"),
                new ParentGroupComponent(530945311),
                new MarketItemGroupComponent(530945311));
            public Entity HammerThor { get; } = new Entity(1317881529, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/hammer/thor"),
                new ParentGroupComponent(1317881529),
                new MarketItemGroupComponent(1317881529));
            public Entity IsisEmerald { get; } = new Entity(2065928272, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/isis/emerald"),
                new ParentGroupComponent(2065928272),
                new MarketItemGroupComponent(2065928272));
            public Entity IsisShine { get; } = new Entity(-132552041, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/isis/shine"),
                new ParentGroupComponent(-132552041),
                new MarketItemGroupComponent(-132552041));
            public Entity IsisStandard { get; } = new Entity(48235025, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/isis/standard"),
                new ParentGroupComponent(48235025),
                new MarketItemGroupComponent(48235025));
            public Entity RailgunGlitch { get; } = new Entity(-1975536348, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/glitch"),
                new ParentGroupComponent(-1975536348),
                new MarketItemGroupComponent(-1975536348));
            public Entity RailgunGloom { get; } = new Entity(-375403918, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/gloom"),
                new ParentGroupComponent(-375403918),
                new MarketItemGroupComponent(-375403918));
            public Entity RailgunGreen { get; } = new Entity(-660457061, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/green"),
                new ParentGroupComponent(-660457061),
                new MarketItemGroupComponent(-660457061));
            public Entity RailgunOrange { get; } = new Entity(1229590166, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/orange"),
                new ParentGroupComponent(1229590166),
                new MarketItemGroupComponent(1229590166));
            public Entity RailgunPaleblue { get; } = new Entity(366763244, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/paleblue"),
                new ParentGroupComponent(366763244),
                new MarketItemGroupComponent(366763244));
            public Entity RailgunPurple { get; } = new Entity(1261498404, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/purple"),
                new ParentGroupComponent(1261498404),
                new MarketItemGroupComponent(1261498404));
            public Entity RailgunSmoke { get; } = new Entity(-1195981301, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/railgun/smoke"),
                new ParentGroupComponent(-1195981301),
                new MarketItemGroupComponent(-1195981301));
            public Entity RicochetAcid { get; } = new Entity(-492753567, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/ricochet/acid"),
                new ParentGroupComponent(-492753567),
                new MarketItemGroupComponent(-492753567));
            public Entity RicochetAurulent { get; } = new Entity(139800007, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/ricochet/aurulent"),
                new ParentGroupComponent(139800007),
                new MarketItemGroupComponent(139800007));
            public Entity RicochetCannonball { get; } = new Entity(-1899861235, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/ricochet/cannonball"),
                new ParentGroupComponent(-1899861235),
                new MarketItemGroupComponent(-1899861235));
            public Entity RicochetCoral { get; } = new Entity(577198848, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/ricochet/coral"),
                new ParentGroupComponent(577198848),
                new MarketItemGroupComponent(577198848));
            public Entity RicochetMoon { get; } = new Entity(-585038633, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/ricochet/moon"),
                new ParentGroupComponent(-585038633),
                new MarketItemGroupComponent(-585038633));
            public Entity RicochetRicsnowball { get; } = new Entity(-1073342998, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/ricochet/ricsnowball"),
                new ParentGroupComponent(-1073342998),
                new MarketItemGroupComponent(-1073342998));
            public Entity ShaftBlue { get; } = new Entity(-1488314890, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/shaft/blue"),
                new ParentGroupComponent(-1488314890),
                new MarketItemGroupComponent(-1488314890));
            public Entity ShaftLighting { get; } = new Entity(-250346840, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/shaft/lighting"),
                new ParentGroupComponent(-250346840),
                new MarketItemGroupComponent(-250346840));
            public Entity ShaftRed { get; } = new Entity(-47995019, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/shaft/red"),
                new ParentGroupComponent(-47995019),
                new MarketItemGroupComponent(-47995019));
            public Entity ShaftStandard { get; } = new Entity(70311513, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/shaft/standard"),
                new ParentGroupComponent(70311513),
                new MarketItemGroupComponent(70311513));
            public Entity SmokyFumes { get; } = new Entity(860375257, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/smoky/fumes"),
                new ParentGroupComponent(860375257),
                new MarketItemGroupComponent(860375257));
            public Entity SmokyLightning { get; } = new Entity(-1260549513, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/smoky/lightning"),
                new ParentGroupComponent(-1260549513),
                new MarketItemGroupComponent(-1260549513));
            public Entity SmokyStandard { get; } = new Entity(-966935184, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/smoky/standard"),
                new ParentGroupComponent(-966935184),
                new MarketItemGroupComponent(-966935184));
            public Entity SmokyThermite { get; } = new Entity(-1165747871, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/smoky/thermite"),
                new ParentGroupComponent(-1165747871),
                new MarketItemGroupComponent(-1165747871));
            public Entity ThunderDragon { get; } = new Entity(-1852311024, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/thunder/dragon"),
                new ParentGroupComponent(-1852311024),
                new MarketItemGroupComponent(-1852311024));
            public Entity ThunderLightning { get; } = new Entity(794635633, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/thunder/lightning"),
                new ParentGroupComponent(794635633),
                new MarketItemGroupComponent(794635633));
            public Entity ThunderStandard { get; } = new Entity(1067800943, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/thunder/standard"),
                new ParentGroupComponent(1067800943),
                new MarketItemGroupComponent(1067800943));
            public Entity TwinsBlue { get; } = new Entity(807172229, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/twins/blue"),
                new ParentGroupComponent(807172229),
                new MarketItemGroupComponent(807172229));
            public Entity TwinsOrange { get; } = new Entity(-1319432295, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/twins/orange"),
                new ParentGroupComponent(-1319432295),
                new MarketItemGroupComponent(-1319432295));
            public Entity TwinsTurquoise { get; } = new Entity(-1173768746, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/twins/turquoise"),
                new ParentGroupComponent(-1173768746),
                new MarketItemGroupComponent(-1173768746));
            public Entity TwinsTwinssnowball { get; } = new Entity(-1092514341, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/twins/twinssnowball"),
                new ParentGroupComponent(-1092514341),
                new MarketItemGroupComponent(-1092514341));
            public Entity TwinsViolet { get; } = new Entity(-1126924822, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/twins/violet"),
                new ParentGroupComponent(-1126924822),
                new MarketItemGroupComponent(-1126924822));
            public Entity TwinsWhite { get; } = new Entity(-728200866, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/twins/white"),
                new ParentGroupComponent(-728200866),
                new MarketItemGroupComponent(-728200866));
            public Entity VulcanAcid { get; } = new Entity(189181410, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/vulcan/acid"),
                new ParentGroupComponent(189181410),
                new MarketItemGroupComponent(189181410));
            public Entity VulcanBlue { get; } = new Entity(189220223, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/vulcan/blue"),
                new ParentGroupComponent(189220223),
                new MarketItemGroupComponent(189220223));
            public Entity VulcanRed { get; } = new Entity(-963712308, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/vulcan/red"),
                new ParentGroupComponent(-963712308),
                new MarketItemGroupComponent(-963712308));
            public Entity VulcanStandard { get; } = new Entity(1322064226, new TemplateAccessor(new ShellMarketItemTemplate(), "garage/shell/vulcan/standard"),
                new ParentGroupComponent(1322064226),
                new MarketItemGroupComponent(1322064226));
        }
    }
}
